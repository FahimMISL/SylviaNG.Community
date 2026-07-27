using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskCommentRepository _taskCommentRepository;
        private readonly ITaskAttachmentRepository _taskAttachmentRepository;
        private readonly ITaskHistoryRepository _taskHistoryRepository;
        private readonly ITaskTagRepository _taskTagRepository;
        private readonly ITaskTagMappingRepository _taskTagMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(
            ITaskRepository taskRepository,
            ITaskCommentRepository taskCommentRepository,
            ITaskAttachmentRepository taskAttachmentRepository,
            ITaskHistoryRepository taskHistoryRepository,
            ITaskTagRepository taskTagRepository,
            ITaskTagMappingRepository taskTagMappingRepository,
            IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _taskCommentRepository = taskCommentRepository;
            _taskAttachmentRepository = taskAttachmentRepository;
            _taskHistoryRepository = taskHistoryRepository;
            _taskTagRepository = taskTagRepository;
            _taskTagMappingRepository = taskTagMappingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TaskCreateRequest request)
        {
            var entity = request.ToEntity();
            await _taskRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.TaskId;
        }

        public async System.Threading.Tasks.Task UpdateAsync(long taskId, TaskUpdateRequest request, long? changedBy)
        {
            var entity = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

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

            await _unitOfWork.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(long taskId)
        {
            var entity = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

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

        public async Task<long> AddCommentAsync(long taskId, TaskCommentAddRequest request)
        {
            _ = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            var entity = request.ToEntity(taskId);
            await _taskCommentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CommentId;
        }

        public async Task<List<TaskCommentResponse>> GetCommentsAsync(long taskId)
        {
            var comments = await _taskCommentRepository.GetByTaskIdAsync(taskId);
            return comments.Select(c => c.ToResponse()).ToList();
        }

        public async Task<long> AddAttachmentAsync(long taskId, TaskAttachmentAddRequest request)
        {
            _ = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            var entity = request.ToEntity(taskId);
            await _taskAttachmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AttachmentId;
        }

        public async System.Threading.Tasks.Task RemoveAttachmentAsync(long taskId, long attachmentId)
        {
            var entity = await _taskAttachmentRepository.GetByIdAsync(attachmentId);
            if (entity == null || entity.TaskId != taskId)
                throw new NotFoundException("TaskAttachment", attachmentId);

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

        public async Task<long> AssignTagAsync(long taskId, TaskTagAssignRequest request)
        {
            _ = await _taskRepository.GetByIdAsync(taskId)
                ?? throw new NotFoundException("Task", taskId);

            var alreadyAssigned = await _taskTagMappingRepository.ExistsAsync(taskId, request.TagId);
            if (alreadyAssigned)
                throw new DuplicateException("TaskTagMapping", "TagId", request.TagId.ToString());

            var entity = new TaskTagMapping { TaskId = taskId, TagId = request.TagId };
            await _taskTagMappingRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.MappingId;
        }

        public async System.Threading.Tasks.Task RemoveTagAsync(long taskId, long tagId)
        {
            var entity = await _taskTagMappingRepository.GetAsync(taskId, tagId)
                ?? throw new NotFoundException("TaskTagMapping", tagId);

            _taskTagMappingRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TaskTagResponse>> GetTagsAsync(long taskId)
        {
            var mappings = await _taskTagMappingRepository.GetByTaskIdAsync(taskId);
            var responses = new List<TaskTagResponse>();

            foreach (var mapping in mappings)
            {
                var tag = await _taskTagRepository.GetByIdAsync(mapping.TagId);
                if (tag != null)
                    responses.Add(tag.ToResponse());
            }

            return responses;
        }
    }
}
