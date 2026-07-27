using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPollVoteRepository : IRepository<PollVote>
    {
        Task<PollVote?> GetByEmployeeAndOptionsAsync(long employeeId, IEnumerable<long> pollOptionIds);
        Task<List<PollVote>> GetByOptionsAsync(IEnumerable<long> pollOptionIds);
    }
}
