using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPollOptionRepository : IRepository<PollOption>
    {
        Task<List<PollOption>> GetByPollIdAsync(long pollId);
    }
}
