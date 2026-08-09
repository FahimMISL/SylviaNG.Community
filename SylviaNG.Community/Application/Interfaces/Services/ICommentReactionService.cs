using SylviaNG.Community.Application.Features.CommentReactions.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ICommentReactionService
    {
        Task<CommentReactionResponse?> AddOrToggleAsync(long commentId, CommentReactionAddRequest request);
        Task<List<CommentReactionResponse>> GetByCommentIdAsync(long commentId);
        Task RemoveAsync(long commentId, long employeeId);
    }
}
