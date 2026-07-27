using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TaskAttachmentRepository : Repository<TaskAttachment>, ITaskAttachmentRepository
    {
        public TaskAttachmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<TaskAttachment>> GetByTaskIdAsync(long taskId)
        {
            return await _dbSet.Where(a => a.TaskId == taskId).OrderBy(a => a.CreatedAt).ToListAsync();
        }
    }
}
