using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskCommentRepository _taskCommentRepository;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        private readonly ITaskHistoryRepository _taskHistoryRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IRecurringTaskRepository _recurringTaskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(
            ITaskRepository taskRepository,
            ITaskCommentRepository taskCommentRepository,
            ITaskAttachmentRepository taskAttachmentRepository,
            ITaskHistoryRepository taskHistoryRepository,
            ITeamRepository teamRepository,
            ITeamMemberRepository teamMemberRepository,
            IRecurringTaskRepository recurringTaskRepository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _taskCommentRepository = taskCommentRepository;
            _taskAttachmentRepository = taskAttachmentRepository;
            _taskHistoryRepository = taskHistoryRepository;
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _recurringTaskRepository = recurringTaskRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>True when the caller supervises the task's own team (US-7.2/7.4 "team's Supervisor").</summary>
        private async Task<bool> IsTeamSupervisorAsync(TaskEntity task, long? callerEmployeeId)
        {
            if (callerEmployeeId == null || task.TeamId == null)
                return false;

            var team = await _teamRepository.GetByIdAsync(task.TeamId.Value);
            return team != null && team.SupervisorId == callerEmployeeId;
        }

        /// <summary>Assigner, assignee, team Supervisor, or HR/Admin - anyone with a legitimate stake in the task
        /// (US-7.8 status updates, US-7.10 comments, US-7.11 attachments all fall under this).</summary>
        private async System.Threading.Tasks.Task EnsureParticipantAccessAsync(TaskEntity task, long? callerEmployeeId, bool isHrOrAdmin)
        {
            if (isHrOrAdmin)
                return;

            if (callerEmployeeId != null && (task.AssignedBy == callerEmployeeId || task.AssignedTo == callerEmployeeId))
                return;

            if (await IsTeamSupervisorAsync(task, callerEmployeeId))
                return;

            throw new ForbiddenException("You do not have access to this task.");
        }

        /// <summary>Assigner, team Supervisor, or HR/Admin - task-management actions the assignee cannot do
        /// (cancelling/deleting the task, tagging it - US-7.4, US-7.13, US-7.14).</summary>
        private async System.Threading.Tasks.Task EnsureManageAccessAsync(TaskEntity task, long? callerEmployeeId, bool isHrOrAdmin)
        {
            if (isHrOrAdmin)
                return;

            if (callerEmployeeId != null && task.AssignedBy == callerEmployeeId)
                return;

            if (await IsTeamSupervisorAsync(task, callerEmployeeId))
                return;

            throw new ForbiddenException("Only this task's assigner, its team's Supervisor, or HR/Admin can do this.");
        }

        public async Task<long> CreateAsync(TaskCreateRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            if (!isHrOrAdmin)
            {
                if (request.TeamId.HasValue)
                {
                    // Team task (US-7.4): only that team's own Supervisor may assign to it.
                    var team = await _teamRepository.GetByIdAsync(request.TeamId.Value)
                        ?? throw new NotFoundException("Team", request.TeamId.Value);

                    if (callerEmployeeId == null || team.SupervisorId != callerEmployeeId)
                        throw new ForbiddenException("Only this team's Supervisor, or HR/Admin, can assign it a task.");
                }
                else
                {
                    // Individual task (US-7.6): caller must at least supervise some team - "Supervisor,
                    // HR, or Admin," per the spec, not any plain Employee.
                    var callerSupervisesATeam = callerEmployeeId != null
                        && await _teamRepository.ExistsBySupervisorIdAsync(callerEmployeeId.Value);

                    if (!callerSupervisesATeam)
                        throw new ForbiddenException("Only a Supervisor, HR, or Admin can assign an individual task.");
                }
            }

            var entity = request.ToEntity();
            await _taskRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await NotifyAssignmentAsync(entity.AssignedTo, entity.TaskId, entity.Title);

            return entity.TaskId;
        }

        /// <summary>US-7.4/7.6: fired once per created task (and once per bulk-reassigned task, US-7.13).</summary>
        private async System.Threading.Tasks.Task NotifyAssignmentAsync(long assignedTo, long taskId, string title)
        {
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = assignedTo,
                Title = $"You've been assigned a task: {title}",
                Category = "Task",
                RelatedEntityType = "Task",
                RelatedEntityId = taskId
            });
        }

        public async System.Threading.Tasks.Task UpdateAsync(long taskId, TaskUpdateRequest request, long? changedBy, bool isHrOrAdmin)
        {
            var entity = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            await EnsureParticipantAccessAsync(entity, changedBy, isHrOrAdmin);

            // Capture pre-update values so we can log what actually changed (insert-only
            // TaskHistory log - see TaskHistory entity / ITaskHistoryRepository).
            var oldStatus = entity.TaskStatus;
            var oldPriority = entity.Priority;
            var oldAssignedTo = entity.AssignedTo;
            var oldDueDate = entity.DueDate;

            entity.ApplyUpdate(request);
            _taskRepository.Update(entity);

            // Falls back to the original assigner when the caller's identity isn't resolvable
            // (e.g. an Admin-type caller with no Employee record - see ICurrentUserService).
            var actualChangedBy = changedBy ?? entity.AssignedBy;

            if (request.Status != null && request.Status != oldStatus)
            {
                await _taskHistoryRepository.AddAsync(new TaskHistory
                {
                    TaskId = taskId,
                    Action = "StatusChanged",
                    OldValue = oldStatus,
                    NewValue = entity.TaskStatus,
                    ChangedBy = actualChangedBy
                });
            }

            if (request.Priority != null && request.Priority != oldPriority)
            {
                await _taskHistoryRepository.AddAsync(new TaskHistory
                {
                    TaskId = taskId,
                    Action = "PriorityChanged",
                    OldValue = oldPriority,
                    NewValue = entity.Priority,
                    ChangedBy = actualChangedBy
                });
            }

            if (request.AssignedTo.HasValue && request.AssignedTo.Value != oldAssignedTo)
            {
                await _taskHistoryRepository.AddAsync(new TaskHistory
                {
                    TaskId = taskId,
                    Action = "AssignedToChanged",
                    OldValue = oldAssignedTo.ToString(),
                    NewValue = entity.AssignedTo.ToString(),
                    ChangedBy = actualChangedBy
                });
            }

            if (request.DueDate.HasValue && request.DueDate != oldDueDate)
            {
                await _taskHistoryRepository.AddAsync(new TaskHistory
                {
                    TaskId = taskId,
                    Action = "DueDateChanged",
                    OldValue = oldDueDate?.ToString("O"),
                    NewValue = entity.DueDate?.ToString("O"),
                    ChangedBy = actualChangedBy
                });
            }

            // US-7.12: completing an instance of a still-active recurring series generates the next one.
            var justCompleted = !string.Equals(oldStatus, "Completed", StringComparison.OrdinalIgnoreCase)
                && string.Equals(entity.TaskStatus, "Completed", StringComparison.OrdinalIgnoreCase);
            if (justCompleted && entity.RecurringTaskId.HasValue)
                await GenerateNextRecurringInstanceAsync(entity);

            await _unitOfWork.SaveChangesAsync();
        }

        private async System.Threading.Tasks.Task GenerateNextRecurringInstanceAsync(TaskEntity completedTask)
        {
            var recurringTask = await _recurringTaskRepository.GetByIdAsync(completedTask.RecurringTaskId!.Value);
            if (recurringTask == null || !recurringTask.IsActive)
                return; // series was cancelled or removed - stop generating.

            var fromDate = completedTask.DueDate ?? DateTime.UtcNow;
            var nextDueDate = RecurringTaskMapper.GetNextOccurrence(fromDate, recurringTask.Frequency, recurringTask.IntervalValue);

            if (recurringTask.EndDate.HasValue && nextDueDate > recurringTask.EndDate.Value)
                return; // next occurrence would fall past the series' end date.

            await _taskRepository.AddAsync(new TaskEntity
            {
                TeamId = completedTask.TeamId,
                AssignedBy = completedTask.AssignedBy,
                AssignedTo = completedTask.AssignedTo,
                RecurringTaskId = completedTask.RecurringTaskId,
                Title = completedTask.Title,
                Description = completedTask.Description,
                Priority = completedTask.Priority,
                TaskStatus = "Assigned",
                DueDate = nextDueDate,
                ReminderDays = completedTask.ReminderDays
            });
        }

        public async System.Threading.Tasks.Task DeleteAsync(long taskId, long? callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            await EnsureManageAccessAsync(entity, callerEmployeeId, isHrOrAdmin);

            _taskRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TaskResponse> GetByIdAsync(long taskId)
        {
            var entity = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<TaskResponse>> GetPaginatedAsync(TaskFilterRequest request)
        {
            var pagedResult = await _taskRepository.GetPaginatedAsync(request);

            return new PagedResult<TaskResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public Task<PagedResult<TaskResponse>> GetMyPaginatedAsync(TaskFilterRequest request, long? callerEmployeeId)
        {
            // No employee record (e.g. an Admin-type caller) has no personal tasks - force an
            // impossible id rather than special-casing, so the result is simply empty.
            request.AssignedTo = callerEmployeeId ?? -1;
            request.AssignedBy = null;
            request.IndividualOnly = false;

            return GetPaginatedAsync(request);
        }

        public Task<PagedResult<TaskResponse>> GetAssignedByMePaginatedAsync(TaskFilterRequest request, long? callerEmployeeId)
        {
            request.AssignedBy = callerEmployeeId ?? -1;
            request.AssignedTo = null;
            request.IndividualOnly = true;

            return GetPaginatedAsync(request);
        }

        public async Task<PagedResult<TaskResponse>> GetTeamPaginatedAsync(long teamId, TaskFilterRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            if (!isHrOrAdmin)
            {
                var team = await _teamRepository.GetByIdAsync(teamId)
                    ?? throw new NotFoundException("Team", teamId);

                var isSupervisor = callerEmployeeId != null && team.SupervisorId == callerEmployeeId;
                var isMember = callerEmployeeId != null && await _teamMemberRepository.ExistsAsync(teamId, callerEmployeeId.Value);

                if (!isSupervisor && !isMember)
                    throw new ForbiddenException("Only this team's Supervisor, its members, or HR/Admin, can view its task board.");
            }

            request.TeamId = teamId;
            request.AssignedTo = null;
            request.AssignedBy = null;
            request.IndividualOnly = false;

            return await GetPaginatedAsync(request);
        }

        public async Task<long> AddCommentAsync(long taskId, TaskCommentAddRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            var task = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            await EnsureParticipantAccessAsync(task, callerEmployeeId, isHrOrAdmin);

            var entity = request.ToEntity(taskId);
            await _taskCommentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // US-7.10: notify whichever of assigner/assignee didn't write the comment (skip if the
            // task is self-assigned and that same person commented - no one else to notify).
            var otherParty = request.EmployeeId == task.AssignedTo ? task.AssignedBy : task.AssignedTo;
            if (otherParty != request.EmployeeId)
            {
                var commenterName = await GetEmployeeNameAsync(request.EmployeeId);
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = otherParty,
                    Title = $"{commenterName} commented on task \"{task.Title}\"",
                    Message = request.Comment,
                    Category = "Task",
                    RelatedEntityType = "Task",
                    RelatedEntityId = taskId
                });
            }

            return entity.CommentId;
        }

        private async Task<string> GetEmployeeNameAsync(long employeeId) =>
            (await _employeeRepository.GetByIdAsync(employeeId))?.EmployeeName ?? "Someone";

        public async Task<List<TaskCommentResponse>> GetCommentsAsync(long taskId)
        {
            var comments = await _taskCommentRepository.GetByTaskIdAsync(taskId);
            return comments.Select(c => c.ToResponse()).ToList();
        }

        public async Task<long> AddAttachmentAsync(long taskId, TaskAttachmentAddRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            var task = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            await EnsureParticipantAccessAsync(task, callerEmployeeId, isHrOrAdmin);

            var entity = request.ToEntity(taskId);
            await _taskAttachmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AttachmentId;
        }

        public async System.Threading.Tasks.Task RemoveAttachmentAsync(long taskId, long attachmentId, long? callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _taskAttachmentRepository.GetByIdAsync(attachmentId);
            if (entity == null || entity.TaskId != taskId)
                throw new NotFoundException("TaskAttachment", attachmentId);

            // US-7.11: the uploader can always remove their own attachment; otherwise it falls
            // back to the same task-management access as other mutating actions.
            if (!isHrOrAdmin && entity.UploadedBy != callerEmployeeId)
            {
                var task = await _taskRepository.GetByIdAsync(taskId)
                    ?? throw new NotFoundException("Task", taskId);

                await EnsureManageAccessAsync(task, callerEmployeeId, isHrOrAdmin);
            }

            _taskAttachmentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TaskAttachmentResponse>> GetAttachmentsAsync(long taskId)
        {
            var attachments = await _taskAttachmentRepository.GetByTaskIdAsync(taskId);
            return attachments.Select(a => a.ToResponse()).ToList();
        }

        public async Task<List<TaskHistoryResponse>> GetHistoryAsync(long taskId)
        {
            var history = await _taskHistoryRepository.GetByTaskIdAsync(taskId);
            return history.Select(h => h.ToResponse()).ToList();
        }

        public async System.Threading.Tasks.Task BulkReassignAsync(List<long> taskIds, long newAssignedTo, long? callerEmployeeId, bool isHrOrAdmin)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var taskId in taskIds)
                {
                    var task = await _taskRepository.GetByIdAsync(taskId)
                        ?? throw new NotFoundException("Task", taskId);

                    await EnsureManageAccessAsync(task, callerEmployeeId, isHrOrAdmin);

                    var oldAssignedTo = task.AssignedTo;
                    task.AssignedTo = newAssignedTo;
                    _taskRepository.Update(task);

                    await _taskHistoryRepository.AddAsync(new TaskHistory
                    {
                        TaskId = taskId,
                        Action = "AssignedToChanged",
                        OldValue = oldAssignedTo.ToString(),
                        NewValue = newAssignedTo.ToString(),
                        ChangedBy = callerEmployeeId ?? task.AssignedBy
                    });

                    // US-7.13: the new assignee is notified for each reassigned task.
                    await NotifyAssignmentAsync(newAssignedTo, taskId, task.Title);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async System.Threading.Tasks.Task BulkCancelAsync(List<long> taskIds, long? callerEmployeeId, bool isHrOrAdmin)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var taskId in taskIds)
                {
                    var task = await _taskRepository.GetByIdAsync(taskId)
                        ?? throw new NotFoundException("Task", taskId);

                    await EnsureManageAccessAsync(task, callerEmployeeId, isHrOrAdmin);

                    _taskRepository.Delete(task);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<TaskReportResult> GenerateReportAsync(long taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            if (!string.Equals(task.TaskStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                throw new FluentValidation.ValidationException("Only a completed task has a downloadable report.");

            var comments = await _taskCommentRepository.GetByTaskIdAsync(taskId);
            var history = await _taskHistoryRepository.GetByTaskIdAsync(taskId);
            var assignedToName = await GetEmployeeNameAsync(task.AssignedTo);
            var assignedByName = await GetEmployeeNameAsync(task.AssignedBy);

            var report = new System.Text.StringBuilder();
            report.AppendLine($"Task Report: {task.Title}");
            report.AppendLine(new string('=', 60));
            report.AppendLine($"Status: {task.TaskStatus}");
            report.AppendLine($"Priority: {task.Priority}");
            report.AppendLine($"Assigned To: {assignedToName}");
            report.AppendLine($"Assigned By: {assignedByName}");
            report.AppendLine($"Due Date: {task.DueDate?.ToString("yyyy-MM-dd") ?? "-"}");
            report.AppendLine($"Created: {(task.CreatedAt.HasValue ? task.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm") + " UTC" : "-")}");

            if (!string.IsNullOrWhiteSpace(task.Description))
            {
                report.AppendLine();
                report.AppendLine("Description:");
                report.AppendLine(task.Description);
            }

            report.AppendLine();
            report.AppendLine($"Comments ({comments.Count})");
            report.AppendLine(new string('-', 60));
            foreach (var comment in comments.OrderBy(c => c.CreatedAt))
            {
                var commenterName = await GetEmployeeNameAsync(comment.EmployeeId);
                report.AppendLine($"[{comment.CreatedAt:yyyy-MM-dd HH:mm}] {commenterName}: {comment.Comment}");
            }

            report.AppendLine();
            report.AppendLine($"History ({history.Count})");
            report.AppendLine(new string('-', 60));
            foreach (var entry in history.OrderBy(h => h.CreatedAt))
            {
                report.AppendLine($"[{entry.CreatedAt:yyyy-MM-dd HH:mm}] {entry.Action}: {entry.OldValue} -> {entry.NewValue}");
            }

            return new TaskReportResult
            {
                Content = System.Text.Encoding.UTF8.GetBytes(report.ToString()),
                FileName = $"task-{taskId}-report.txt",
                ContentType = "text/plain"
            };
        }
    }
}
