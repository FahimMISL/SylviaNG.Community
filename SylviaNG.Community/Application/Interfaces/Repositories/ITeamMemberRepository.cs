using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITeamMemberRepository : IRepository<TeamMember>
    {
        Task<bool> ExistsAsync(long teamId, long employeeId);
        Task<TeamMember?> GetAsync(long teamId, long employeeId);
        Task<List<TeamMember>> GetByTeamIdAsync(long teamId);

        /// <summary>Distinct active employee ids across the given teams - "Team"-scoped election eligibility.</summary>
        Task<List<long>> GetActiveEmployeeIdsByTeamIdsAsync(IEnumerable<long> teamIds);
    }
}
