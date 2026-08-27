using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionRepository : IRepository<Election>
    {
        Task<PagedResult<Election>> GetPaginatedAsync(PagedRequest request);

        /// <summary>Elections currently in the given status - used to list Open elections (US-9.8) and auto-close expired ones.</summary>
        Task<List<Election>> GetByStatusAsync(string status);
    }
}
