using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<PagedResult<Group>> GetPaginatedAsync(PagedRequest request);
    }
}
