using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class TeamMemberRepository : Repository<TeamMember>, ITeamMemberRepository
    {
        public TeamMemberRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long teamId, long employeeId)
        {
            return await _dbSet.AnyAsync(tm => tm.TeamId == teamId && tm.EmployeeId == employeeId && tm.IsActive);
        }

        public async Task<TeamMember?> GetAsync(long teamId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.EmployeeId == employeeId);
        }

        public async Task<List<TeamMember>> GetByTeamIdAsync(long teamId)
        {
            return await _dbSet.Where(tm => tm.TeamId == teamId && tm.IsActive).ToListAsync();
        }
    }
}
