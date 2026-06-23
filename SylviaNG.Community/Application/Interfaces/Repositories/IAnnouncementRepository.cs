using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IAnnouncementRepository : IRepository<Announcement>
    {
        Task<Announcement?> GetByTitleAndSiteIdAsync(string title, long siteId);
        Task<bool> ExistsByTitleAndSiteIdAsync(string title, long siteId, long? excludeId = null);
        Task<PagedResult<Announcement>> GetPaginatedAsync(PagedRequest request);
        Task<List<Announcement>> GetActiveBySiteIdAsync(long siteId);
    }
}