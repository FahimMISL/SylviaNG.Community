using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PollOptionRepository : Repository<PollOption>, IPollOptionRepository
    {
        public PollOptionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<PollOption>> GetByPollIdAsync(long pollId)
        {
            return await _dbSet.Where(o => o.PollId == pollId).ToListAsync();
        }
    }
}
