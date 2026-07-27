using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class EmployeeBadgeRepository : Repository<EmployeeBadge>, IEmployeeBadgeRepository
    {
        public EmployeeBadgeRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<EmployeeBadge>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.Where(eb => eb.EmployeeId == employeeId).ToListAsync();
        }
    }
}
