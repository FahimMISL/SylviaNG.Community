using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TaskCommentRepository : Repository<TaskComment>, ITaskCommentRepository
    {
        public TaskCommentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<TaskComment>> GetByTaskIdAsync(long taskId)
        {
            return await _dbSet.Where(c => c.TaskId == taskId).OrderBy(c => c.CreatedAt).ToListAsync();
        }
    }
}
