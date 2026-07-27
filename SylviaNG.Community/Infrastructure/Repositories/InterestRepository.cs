using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class InterestRepository : Repository<Interest>, IInterestRepository
    {
        public InterestRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(i => i.Name == name && (!excludeId.HasValue || i.InterestId != excludeId.Value));
        }

        public async Task<PagedResult<Interest>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Interest.Name) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
