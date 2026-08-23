using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<PagedResult<Team>> GetPaginatedAsync(PagedRequest request);

        /// <summary>True when the employee currently supervises at least one active team - the
        /// spec's definition of "Supervisor" (not a separate role/claim, see US-7.1).</summary>
        Task<bool> ExistsBySupervisorIdAsync(long employeeId);
    }
}
