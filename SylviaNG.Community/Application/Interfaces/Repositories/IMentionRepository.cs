using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IMentionRepository : IRepository<Mention>
    {
        Task<PagedResult<Mention>> GetPaginatedForEmployeeAsync(long mentionedEmployeeId, PagedRequest request);

        /// <summary>
        /// All mentions recorded against a specific Post or PostComment - used to resolve which
        /// employeeId each "@Name" occurrence in that content refers to when rendering it.
        /// </summary>
        Task<List<Mention>> GetByEntityAsync(string entityType, long entityId);
    }
}
