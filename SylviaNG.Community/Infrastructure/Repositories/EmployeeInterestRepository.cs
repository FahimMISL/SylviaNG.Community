using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class EmployeeInterestRepository : Repository<EmployeeInterest>, IEmployeeInterestRepository
    {
        public EmployeeInterestRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long employeeId, long interestId)
        {
            return await _dbSet.AnyAsync(ei => ei.EmployeeId == employeeId && ei.InterestId == interestId);
        }

        public async Task<EmployeeInterest?> GetAsync(long employeeId, long interestId)
        {
            return await _dbSet.FirstOrDefaultAsync(ei => ei.EmployeeId == employeeId && ei.InterestId == interestId);
        }

        public async Task<List<EmployeeInterest>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.Where(ei => ei.EmployeeId == employeeId).ToListAsync();
        }
    }
}
