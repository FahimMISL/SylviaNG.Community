using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IActivityLogRepository : IRepository<ActivityLog>
    {
        Task<PagedResult<ActivityLog>> GetPaginatedAsync(PagedRequest request);
    }
}
