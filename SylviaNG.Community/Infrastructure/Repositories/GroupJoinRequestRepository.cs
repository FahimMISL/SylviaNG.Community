using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class GroupJoinRequestRepository : Repository<GroupJoinRequest>, IGroupJoinRequestRepository
    {
        public GroupJoinRequestRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<GroupJoinRequest?> GetPendingAsync(long groupId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(r =>
                r.GroupId == groupId && r.EmployeeId == employeeId && r.Status == GroupJoinRequestStatusEnum.Pending);
        }

        public async Task<List<GroupJoinRequest>> GetPendingByGroupIdAsync(long groupId)
        {
            return await _dbSet
                .Where(r => r.GroupId == groupId && r.Status == GroupJoinRequestStatusEnum.Pending)
                .ToListAsync();
        }
    }
}
