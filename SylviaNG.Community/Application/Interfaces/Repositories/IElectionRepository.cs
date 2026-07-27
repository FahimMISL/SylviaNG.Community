using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionRepository : IRepository<Election>
    {
        Task<PagedResult<Election>> GetPaginatedAsync(PagedRequest request);
    }
}
