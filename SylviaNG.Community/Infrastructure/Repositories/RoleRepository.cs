using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(r => r.Name == name && (!excludeId.HasValue || r.RoleId != excludeId.Value));
        }

        public async Task<PagedResult<Role>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Role.Name), nameof(Role.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
