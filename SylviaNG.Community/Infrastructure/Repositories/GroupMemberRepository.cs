using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class GroupMemberRepository : Repository<GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<GroupMember?> GetAsync(long groupId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.EmployeeId == employeeId);
        }

        public async Task<GroupMember?> GetActiveAsync(long groupId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.EmployeeId == employeeId && gm.IsActive);
        }

        public async Task<GroupMember?> GetActiveCreatorAsync(long groupId)
        {
            return await _dbSet.FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.IsActive && gm.Role == GroupMemberRoleEnum.Creator);
        }

        public async Task<List<GroupMember>> GetActiveByGroupIdAsync(long groupId)
        {
            return await _dbSet.Where(gm => gm.GroupId == groupId && gm.IsActive).ToListAsync();
        }

        public async Task<List<GroupMember>> GetActiveByRolesAsync(long groupId, params GroupMemberRoleEnum[] roles)
        {
            return await _dbSet
                .Where(gm => gm.GroupId == groupId && gm.IsActive && roles.Contains(gm.Role))
                .ToListAsync();
        }

        public async Task<Dictionary<long, int>> GetActiveMemberCountsAsync(IEnumerable<long> groupIds)
        {
            return await _dbSet
                .Where(gm => gm.IsActive && groupIds.Contains(gm.GroupId))
                .GroupBy(gm => gm.GroupId)
                .Select(g => new { GroupId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.GroupId, x => x.Count);
        }

        public async Task<List<long>> GetActiveGroupIdsByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet
                .Where(gm => gm.EmployeeId == employeeId && gm.IsActive)
                .Select(gm => gm.GroupId)
                .ToListAsync();
        }
    }
}
