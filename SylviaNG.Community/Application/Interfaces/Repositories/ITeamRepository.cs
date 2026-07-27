using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<PagedResult<Team>> GetPaginatedAsync(PagedRequest request);
    }
}
