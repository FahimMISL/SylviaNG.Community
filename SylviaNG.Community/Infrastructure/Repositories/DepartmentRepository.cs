using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(d => d.Name == name && (!excludeId.HasValue || d.DepartmentId != excludeId.Value));
        }

        public async Task<PagedResult<Department>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(Department.Name), nameof(Department.Code), nameof(Department.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
