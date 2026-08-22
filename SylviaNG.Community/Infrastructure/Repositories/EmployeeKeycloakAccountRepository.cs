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

        public async Task<HashSet<long>> GetEmployeeIdsWithAccountsAsync(IEnumerable<long> employeeIds)
        {
            var ids = employeeIds.ToList();
            if (ids.Count == 0)
                return new HashSet<long>();

            var existing = await _dbSet.Where(e => ids.Contains(e.EmployeeId)).Select(e => e.EmployeeId).ToListAsync();
            return existing.ToHashSet();
        }
    }
}
