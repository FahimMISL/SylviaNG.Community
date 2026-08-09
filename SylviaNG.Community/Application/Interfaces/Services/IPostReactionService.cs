using SylviaNG.Community.Application.Features.PostReactions.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IPostReactionService
    {
        Task<PostReactionResponse?> AddOrToggleAsync(long postId, PostReactionAddRequest request);
        Task<List<PostReactionResponse>> GetByPostIdAsync(long postId);
        Task RemoveAsync(long postId, long employeeId);
    }
}
