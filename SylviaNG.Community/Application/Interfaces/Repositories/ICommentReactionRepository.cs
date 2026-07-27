using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ICommentReactionRepository : IRepository<CommentReaction>
    {
        Task<CommentReaction?> GetAsync(long commentId, long employeeId);
        Task<List<CommentReaction>> GetByCommentIdAsync(long commentId);
    }
}
