using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ITeamService
    {
        /// <summary>Caller must be HR/Admin, or already supervise at least one active team (US-7.1).</summary>
        Task<long> CreateAsync(TeamCreateRequest request, long callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, or HR/Admin (US-7.2).</summary>
        Task UpdateAsync(long teamId, TeamUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, or HR/Admin (US-7.2).</summary>
        Task DeleteAsync(long teamId, long callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, an active member, or HR/Admin.</summary>
        Task<TeamResponse> GetByIdAsync(long teamId, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Unscoped for HR/Admin; otherwise restricted to teams the caller supervises or is an active member of.</summary>
        Task<PagedResult<TeamResponse>> GetPaginatedAsync(PagedRequest request, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, or HR/Admin (US-7.2).</summary>
        Task<long> AddMemberAsync(long teamId, TeamMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, or HR/Admin (US-7.2).</summary>
        Task RemoveMemberAsync(long teamId, long employeeId, long callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, an active member, or HR/Admin.</summary>
        Task<List<TeamMemberResponse>> GetMembersAsync(long teamId, long? callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Open read: any authenticated caller may look up an employee's active team
        /// memberships (used to show team affiliation elsewhere, e.g. election candidate lists) -
        /// unlike GetByIdAsync/GetMembersAsync, this is not gated to supervisor/member/HR-admin.</summary>
        Task<List<TeamResponse>> GetTeamsByEmployeeIdAsync(long employeeId);
    }
}
