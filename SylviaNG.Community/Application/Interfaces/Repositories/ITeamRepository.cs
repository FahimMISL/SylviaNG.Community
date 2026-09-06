using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

        /// <summary>When scopeToEmployeeId is set (regular employee), restricts to teams that
        /// employee supervises or is an active member of - via memberTeamIds, since membership
        /// lives in a separate table. Pass both null for an unscoped (HR/Admin) list.</summary>
        Task<PagedResult<Team>> GetPaginatedAsync(PagedRequest request, long? scopeToEmployeeId, IEnumerable<long>? memberTeamIds);

        /// <summary>True when the employee currently supervises at least one active team - the
        /// spec's definition of "Supervisor" (not a separate role/claim, see US-7.1).</summary>
        Task<bool> ExistsBySupervisorIdAsync(long employeeId);

        /// <summary>Distinct supervisor employee ids for the given active teams - a team's
        /// supervisor isn't necessarily also a TeamMember row, so this is resolved separately
        /// from GetActiveEmployeeIdsByTeamIdsAsync (see ElectionEligibilityService.ResolveCandidateEmployeeIdsAsync).</summary>
        Task<List<long>> GetSupervisorIdsByTeamIdsAsync(IEnumerable<long> teamIds);
    }
}
