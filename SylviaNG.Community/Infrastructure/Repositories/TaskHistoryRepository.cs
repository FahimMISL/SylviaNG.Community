using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TaskHistoryRepository : Repository<TaskHistory>, ITaskHistoryRepository
    {
        public TaskHistoryRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<TaskHistory>> GetByTaskIdAsync(long taskId)
        {
            return await _dbSet.Where(h => h.TaskId == taskId).OrderByDescending(h => h.CreatedAt).ToListAsync();
        }
    }
}
