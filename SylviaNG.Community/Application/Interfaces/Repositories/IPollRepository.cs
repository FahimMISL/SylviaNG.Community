using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPollRepository : IRepository<Poll>
    {
        Task<Poll?> GetByPostIdAsync(long postId);
        Task<bool> ExistsByPostIdAsync(long postId);
    }
}
