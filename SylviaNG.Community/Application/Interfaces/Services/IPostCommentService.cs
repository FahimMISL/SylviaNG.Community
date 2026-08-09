using SylviaNG.Community.Application.Features.PostComments.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IPostCommentService
    {
        Task<long> AddAsync(long postId, PostCommentAddRequest request);
        Task<List<PostCommentResponse>> GetByPostIdAsync(long postId);
        Task UpdateAsync(long postId, long commentId, PostCommentUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin);
        Task DeleteAsync(long postId, long commentId, long callerEmployeeId, bool isHrOrAdmin);
    }
}
