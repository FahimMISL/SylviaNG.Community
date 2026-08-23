using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ITaskService
    {
        /// <summary>Caller must be the target team's Supervisor, or HR/Admin (US-7.4).</summary>
        Task<long> CreateAsync(TaskCreateRequest request, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>
        /// Caller must be the task's assigner or assignee, its team's Supervisor, or HR/Admin.
        /// Applies the update and, when Status/Priority/AssignedTo/DueDate actually change,
        /// writes one TaskHistory row per changed field (insert-only log - see TaskHistory).
        /// </summary>
        Task UpdateAsync(long taskId, TaskUpdateRequest request, long? changedBy, bool isHrOrAdmin);

        /// <summary>Caller must be the task's assigner, its team's Supervisor, or HR/Admin (not the assignee).</summary>
        Task DeleteAsync(long taskId, long? callerEmployeeId, bool isHrOrAdmin);

        Task<TaskResponse> GetByIdAsync(long taskId);

        /// <summary>Unscoped - trusts the caller-supplied filter as-is. Restricted to HR/Admin at the
        /// controller level; use the scoped variants below for anyone else (closes an IDOR-style gap
        /// where a client-supplied AssignedTo/AssignedBy could read anyone's tasks).</summary>
        Task<PagedResult<TaskResponse>> GetPaginatedAsync(TaskFilterRequest request);

        /// <summary>US-7.8: tasks assigned to the caller. AssignedTo is forced server-side, never trusted from the request.</summary>
        Task<PagedResult<TaskResponse>> GetMyPaginatedAsync(TaskFilterRequest request, long? callerEmployeeId);

        /// <summary>US-7.7: individual (non-team) tasks the caller assigned to others. AssignedBy/TeamId are forced server-side.</summary>
        Task<PagedResult<TaskResponse>> GetAssignedByMePaginatedAsync(TaskFilterRequest request, long? callerEmployeeId);

        /// <summary>US-7.5: a team's task board. Caller must be that team's Supervisor, or HR/Admin.</summary>
        Task<PagedResult<TaskResponse>> GetTeamPaginatedAsync(long teamId, TaskFilterRequest request, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be the task's assigner or assignee, its team's Supervisor, or HR/Admin.</summary>
        Task<long> AddCommentAsync(long taskId, TaskCommentAddRequest request, long? callerEmployeeId, bool isHrOrAdmin);
        Task<List<TaskCommentResponse>> GetCommentsAsync(long taskId);

        /// <summary>Caller must be the task's assigner or assignee, its team's Supervisor, or HR/Admin.</summary>
        Task<long> AddAttachmentAsync(long taskId, TaskAttachmentAddRequest request, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be the attachment's uploader, the task's assigner, its team's Supervisor, or HR/Admin.</summary>
        Task RemoveAttachmentAsync(long taskId, long attachmentId, long? callerEmployeeId, bool isHrOrAdmin);
        Task<List<TaskAttachmentResponse>> GetAttachmentsAsync(long taskId);

        Task<List<TaskHistoryResponse>> GetHistoryAsync(long taskId);

        /// <summary>US-7.13: reassign multiple tasks to a new assignee in one transaction. Each task is
        /// re-checked against the manage-access rule (assigner/team Supervisor/HR-Admin) individually.</summary>
        Task BulkReassignAsync(List<long> taskIds, long newAssignedTo, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>US-7.13: permanently cancel (remove) multiple tasks in one transaction.</summary>
        Task BulkCancelAsync(List<long> taskIds, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>US-7.8: a downloadable summary for a completed task (title, assignee, dates, comments,
        /// history). Throws FluentValidation.ValidationException (mapped to 400) if the task isn't Completed.</summary>
        Task<TaskReportResult> GenerateReportAsync(long taskId);
    }
}
