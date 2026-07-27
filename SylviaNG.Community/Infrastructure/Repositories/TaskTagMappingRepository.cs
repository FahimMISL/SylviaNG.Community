using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TaskTagMappingRepository : Repository<TaskTagMapping>, ITaskTagMappingRepository
    {
        public TaskTagMappingRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long taskId, long tagId)
        {
            return await _dbSet.AnyAsync(m => m.TaskId == taskId && m.TagId == tagId);
        }

        public async Task<TaskTagMapping?> GetAsync(long taskId, long tagId)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.TaskId == taskId && m.TagId == tagId);
        }

        public async Task<List<TaskTagMapping>> GetByTaskIdAsync(long taskId)
        {
            return await _dbSet.Where(m => m.TaskId == taskId).ToListAsync();
        }
    }
}
