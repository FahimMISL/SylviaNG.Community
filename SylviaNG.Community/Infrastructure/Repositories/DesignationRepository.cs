using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class DesignationRepository : Repository<Designation>, IDesignationRepository
    {
        public DesignationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(d => d.Name == name && (!excludeId.HasValue || d.DesignationId != excludeId.Value));
        }

        public async Task<PagedResult<Designation>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Designation.Name), nameof(Designation.Grade), nameof(Designation.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
