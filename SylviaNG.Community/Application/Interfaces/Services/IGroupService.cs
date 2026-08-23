using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IGroupService
    {
        Task<long> CreateAsync(GroupCreateRequest request, long callerEmployeeId);
        Task UpdateAsync(long groupId, GroupUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin);
        Task DeleteAsync(long groupId, long callerEmployeeId, bool isHrOrAdmin);
        Task<GroupResponse> GetByIdAsync(long groupId);
        Task<PagedResult<GroupResponse>> GetPaginatedAsync(PagedRequest request);
        Task<List<GroupResponse>> GetMyGroupsAsync(long employeeId);

        Task JoinAsync(long groupId, long callerEmployeeId);
        Task<long> RequestToJoinAsync(long groupId, long callerEmployeeId);
        Task ApproveJoinRequestAsync(long groupJoinRequestId, long callerEmployeeId, bool isHrOrAdmin);
        Task RejectJoinRequestAsync(long groupJoinRequestId, long callerEmployeeId, bool isHrOrAdmin);
        Task<List<GroupJoinRequestResponse>> GetPendingJoinRequestsAsync(long groupId, long callerEmployeeId, bool isHrOrAdmin);

        Task<long> AddMemberAsync(long groupId, GroupMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin);
        Task RemoveMemberAsync(long groupId, long employeeId, long callerEmployeeId, bool isHrOrAdmin);
        Task ChangeMemberRoleAsync(long groupId, GroupMemberRoleChangeRequest request, long callerEmployeeId, bool isHrOrAdmin);
        Task LeaveAsync(long groupId, long callerEmployeeId);
        Task<List<GroupMemberResponse>> GetMembersAsync(long groupId);

        Task<PagedResult<PostResponse>> GetPostsAsync(long groupId, PostFilterRequest request, long callerEmployeeId, bool isHrOrAdmin);
    }
}
