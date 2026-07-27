using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<TaskEntity>> GetPaginatedAsync(TaskFilterRequest request)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(t => t.TaskStatus == request.Status);

            if (request.AssignedTo.HasValue)
                query = query.Where(t => t.AssignedTo == request.AssignedTo.Value);

            if (request.TeamId.HasValue)
                query = query.Where(t => t.TeamId == request.TeamId.Value);

            request.SearchProperties ??= new[] { nameof(TaskEntity.Title), nameof(TaskEntity.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
