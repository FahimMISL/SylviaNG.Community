using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IInterestRepository : IRepository<Interest>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<PagedResult<Interest>> GetPaginatedAsync(PagedRequest request);
    }
}
