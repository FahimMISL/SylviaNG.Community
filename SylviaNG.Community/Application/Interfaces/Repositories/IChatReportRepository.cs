using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IChatReportRepository : IRepository<ChatReport>
    {
        Task<PagedResult<ChatReport>> GetPaginatedAsync(PagedRequest request);
    }
}
