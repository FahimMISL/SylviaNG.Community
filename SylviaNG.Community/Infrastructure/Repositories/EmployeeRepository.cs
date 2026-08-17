using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByEmailAsync(string email, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(e => e.Email == email && (!excludeId.HasValue || e.EmployeeId != excludeId.Value));
        }

        public async Task<int> CountActiveAsync()
        {
            return await _dbSet.CountAsync(e => e.IsActive);
        }

        public async Task<PagedResult<Employee>> GetPaginatedAsync(EmployeeFilterRequest request, bool activeOnly)
        {
            var query = _dbSet.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(e => e.IsActive);
            }
            else if (request.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == request.IsActive.Value);
            }

            if (request.DepartmentId.HasValue)
                query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);

            if (request.SiteId.HasValue)
                query = query.Where(e => e.SiteId == request.SiteId.Value);

            if (request.DesignationId.HasValue)
                query = query.Where(e => e.DesignatioId == request.DesignationId.Value);

            // Default search fields so callers only need to send SearchTerm (US-1.2: name/code/email/skill).
            request.SearchProperties ??= new[]
            {
                nameof(Employee.EmployeeName),
                nameof(Employee.EmployeeCode),
                nameof(Employee.Email),
                nameof(Employee.Skills)
            };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
