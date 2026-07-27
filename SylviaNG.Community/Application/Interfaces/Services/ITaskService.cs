using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<long> CreateAsync(TaskCreateRequest request);

        /// <summary>
        /// Applies the update and, when Status/Priority/AssignedTo/DueDate actually change,
        /// writes one TaskHistory row per changed field (insert-only log - see TaskHistory).
        /// </summary>
        Task UpdateAsync(long taskId, TaskUpdateRequest request, long? changedBy);
        Task DeleteAsync(long taskId);
        Task<TaskResponse> GetByIdAsync(long taskId);
        Task<PagedResult<TaskResponse>> GetPaginatedAsync(TaskFilterRequest request);

        Task<long> AddCommentAsync(long taskId, TaskCommentAddRequest request);
        Task<List<TaskCommentResponse>> GetCommentsAsync(long taskId);

        Task<long> AddAttachmentAsync(long taskId, TaskAttachmentAddRequest request);
        Task RemoveAttachmentAsync(long taskId, long attachmentId);
        Task<List<TaskAttachmentResponse>> GetAttachmentsAsync(long taskId);

        Task<List<TaskHistoryResponse>> GetHistoryAsync(long taskId);

        Task<long> AssignTagAsync(long taskId, TaskTagAssignRequest request);
        Task RemoveTagAsync(long taskId, long tagId);
        Task<List<TaskTagResponse>> GetTagsAsync(long taskId);
    }
}
