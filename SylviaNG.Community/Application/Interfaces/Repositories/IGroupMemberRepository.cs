using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IGroupMemberRepository : IRepository<GroupMember>
    {
        /// <summary>Any status (active or previously-removed) row for this pair, so a rejoin can reactivate it instead of inserting a duplicate.</summary>
        Task<GroupMember?> GetAsync(long groupId, long employeeId);
        Task<GroupMember?> GetActiveAsync(long groupId, long employeeId);
        Task<GroupMember?> GetActiveCreatorAsync(long groupId);
        Task<List<GroupMember>> GetActiveByGroupIdAsync(long groupId);
        Task<List<GroupMember>> GetActiveByRolesAsync(long groupId, params GroupMemberRoleEnum[] roles);
        Task<Dictionary<long, int>> GetActiveMemberCountsAsync(IEnumerable<long> groupIds);
        Task<List<long>> GetActiveGroupIdsByEmployeeIdAsync(long employeeId);
    }
}
