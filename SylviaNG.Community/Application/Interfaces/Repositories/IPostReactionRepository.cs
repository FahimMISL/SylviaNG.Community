using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPostReactionRepository : IRepository<PostReaction>
    {
        Task<PostReaction?> GetAsync(long postId, long employeeId);
        Task<List<PostReaction>> GetByPostIdAsync(long postId);
    }
}
