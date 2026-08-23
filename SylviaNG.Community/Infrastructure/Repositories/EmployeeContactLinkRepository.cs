using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class EmployeeContactLinkRepository : Repository<EmployeeContactLink>, IEmployeeContactLinkRepository
    {
        public EmployeeContactLinkRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<EmployeeContactLink>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.Where(e => e.EmployeeId == employeeId).ToListAsync();
        }
    }
}
