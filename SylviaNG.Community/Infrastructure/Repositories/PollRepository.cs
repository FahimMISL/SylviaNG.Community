using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PollRepository : Repository<Poll>, IPollRepository
    {
        public PollRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<Poll?> GetByPostIdAsync(long postId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<bool> ExistsByPostIdAsync(long postId)
        {
            return await _dbSet.AnyAsync(p => p.PostId == postId);
        }
    }
}
