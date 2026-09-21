using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ElectionRepository : Repository<Election>, IElectionRepository
    {
        public ElectionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Election>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Election.Title), nameof(Election.Description) };

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<Election>> GetByStatusAsync(string status)
        {
            // Most recently published first. The UpdatedAt/CreatedAt fallbacks only matter for rows
            // published before PublishedAt existed and not caught by the migration's backfill.
            return await _dbSet
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.PublishedAt ?? e.UpdatedAt ?? e.CreatedAt)
                .ThenByDescending(e => e.ElectionId)
                .ToListAsync();
        }
    }
}
