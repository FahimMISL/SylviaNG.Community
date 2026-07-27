using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PostReactionRepository : Repository<PostReaction>, IPostReactionRepository
    {
        public PostReactionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PostReaction?> GetAsync(long postId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.PostId == postId && r.EmployeeId == employeeId);
        }

        public async Task<List<PostReaction>> GetByPostIdAsync(long postId)
        {
            return await _dbSet.Where(r => r.PostId == postId).ToListAsync();
        }
    }
}
