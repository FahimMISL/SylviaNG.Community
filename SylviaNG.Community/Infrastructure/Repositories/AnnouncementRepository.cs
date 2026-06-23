using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<Announcement?> GetByTitleAndSiteIdAsync(string title, long siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Title == title && a.SiteId == siteId);
        }

        public async Task<bool> ExistsByTitleAndSiteIdAsync(string title, long siteId, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(a => a.Title == title && a.SiteId == siteId && (!excludeId.HasValue || a.AnnouncementId != excludeId.Value));
        }

        public async Task<PagedResult<Announcement>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<Announcement>> GetActiveBySiteIdAsync(long siteId)
        {
            return await _dbSet
                .Where(a => a.SiteId == siteId && a.IsActive)
                .ToListAsync();
        }
    }
}