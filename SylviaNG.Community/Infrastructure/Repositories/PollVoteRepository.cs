using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PollVoteRepository : Repository<PollVote>, IPollVoteRepository
    {
        public PollVoteRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PollVote?> GetByEmployeeAndOptionsAsync(long employeeId, IEnumerable<long> pollOptionIds)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.EmployeeId == employeeId && pollOptionIds.Contains(v.PollOptionId));
        }

        public async Task<List<PollVote>> GetByOptionsAsync(IEnumerable<long> pollOptionIds)
        {
            return await _dbSet.Where(v => pollOptionIds.Contains(v.PollOptionId)).ToListAsync();
        }
    }
}
