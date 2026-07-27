using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ITaskTagRepository : IRepository<TaskTag>
    {
        Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
        Task<PagedResult<TaskTag>> GetPaginatedAsync(PagedRequest request);
    }
}
