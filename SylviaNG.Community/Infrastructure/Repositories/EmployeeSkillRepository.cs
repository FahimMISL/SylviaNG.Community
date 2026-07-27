using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class EmployeeSkillRepository : Repository<EmployeeSkill>, IEmployeeSkillRepository
    {
        public EmployeeSkillRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long employeeId, long skillId)
        {
            return await _dbSet.AnyAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId);
        }

        public async Task<EmployeeSkill?> GetAsync(long employeeId, long skillId)
        {
            return await _dbSet.FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId);
        }

        public async Task<List<EmployeeSkill>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.Where(es => es.EmployeeId == employeeId).ToListAsync();
        }
    }
}
