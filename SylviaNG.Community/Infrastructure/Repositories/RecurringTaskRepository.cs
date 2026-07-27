using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class RecurringTaskRepository : Repository<RecurringTask>, IRecurringTaskRepository
    {
        public RecurringTaskRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<RecurringTask>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(RecurringTask.Frequency) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
