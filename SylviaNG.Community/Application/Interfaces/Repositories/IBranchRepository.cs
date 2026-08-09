using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IBranchRepository : IRepository<Branch>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<PagedResult<Branch>> GetPaginatedAsync(PagedRequest request);
    }
}
