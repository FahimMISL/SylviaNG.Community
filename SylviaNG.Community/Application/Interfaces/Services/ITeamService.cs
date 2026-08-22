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

        Task<TeamResponse> GetByIdAsync(long teamId);
        Task<PagedResult<TeamResponse>> GetPaginatedAsync(PagedRequest request);

        /// <summary>Caller must be this team's Supervisor, or HR/Admin (US-7.2).</summary>
        Task<long> AddMemberAsync(long teamId, TeamMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin);

        /// <summary>Caller must be this team's Supervisor, or HR/Admin (US-7.2).</summary>
        Task RemoveMemberAsync(long teamId, long employeeId, long callerEmployeeId, bool isHrOrAdmin);

        Task<List<TeamMemberResponse>> GetMembersAsync(long teamId);
    }
}
