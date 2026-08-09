using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TaskTagRepository : Repository<TaskTag>, ITaskTagRepository
    {
        public TaskTagRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet.AnyAsync(t => t.Name == name && (!excludeId.HasValue || t.TagId != excludeId.Value));
        }

        public async Task<PagedResult<TaskTag>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(TaskTag.Name), nameof(TaskTag.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
