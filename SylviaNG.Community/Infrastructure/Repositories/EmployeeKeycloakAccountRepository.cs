using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class EmployeeKeycloakAccountRepository : Repository<EmployeeKeycloakAccount>, IEmployeeKeycloakAccountRepository
    {
        public EmployeeKeycloakAccountRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.AnyAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<EmployeeKeycloakAccount?> GetByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<EmployeeKeycloakAccount?> GetByKeycloakUserIdAsync(string keycloakUserId)
        {
            // IgnoreQueryFilters: this lookup resolves identity itself (login, and any request
            // whose token is missing "employee_id" - see EmployeeIdentityEnrichmentMiddleware), so
            // it runs before tenant context can be established from the caller's own claims - at
            // login time in particular, the request is anonymous, so CurrentTenantId is always ""
            // and the normal TenantId filter would silently find nothing. Safe to bypass here
            // specifically because KeycloakUserId is Keycloak's own globally unique subject, not
            // something that could plausibly collide across tenants.
            return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.KeycloakUserId == keycloakUserId);
        }

        public async Task<HashSet<long>> GetEmployeeIdsWithAccountsAsync(IEnumerable<long> employeeIds)
        {
            var ids = employeeIds.ToList();
            if (ids.Count == 0)
                return new HashSet<long>();

            var existing = await _dbSet.Where(e => ids.Contains(e.EmployeeId)).Select(e => e.EmployeeId).ToListAsync();
            return existing.ToHashSet();
        }

        public async Task<List<long>> GetEmployeeIdsByRolesAsync(IEnumerable<string> roles)
        {
            var roleList = roles.ToList();
            return await _dbSet.Where(e => e.IsActive && roleList.Contains(e.AssignedRole)).Select(e => e.EmployeeId).ToListAsync();
        }
    }
}
