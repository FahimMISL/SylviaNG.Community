using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IContentReportRepository : IRepository<ContentReport>
    {
        Task<PagedResult<ContentReport>> GetPaginatedAsync(PagedRequest request);
    }
}
