using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ITeamService
    {
        Task<long> CreateAsync(TeamCreateRequest request);
        Task UpdateAsync(long teamId, TeamUpdateRequest request);
        Task DeleteAsync(long teamId);
        Task<TeamResponse> GetByIdAsync(long teamId);
        Task<PagedResult<TeamResponse>> GetPaginatedAsync(PagedRequest request);
        Task<long> AddMemberAsync(long teamId, TeamMemberAddRequest request);
        Task RemoveMemberAsync(long teamId, long employeeId);
        Task<List<TeamMemberResponse>> GetMembersAsync(long teamId);
    }
}
