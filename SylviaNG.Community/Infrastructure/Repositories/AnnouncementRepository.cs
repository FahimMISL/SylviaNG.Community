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
                .FirstOrDefaultAsync(j => j.Title == title && j.SiteId == siteId);
        }

        public async Task<bool> ExistsByTitleAndSiteIdAsync(string title, long siteId, long? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(j => j.Title == title && j.SiteId == siteId && (!excludeId.HasValue || j.AnnouncementId != excludeId.Value));
        }

        public async Task<PagedResult<Announcement>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet
                .Include(j => j.Applications)
                .AsQueryable();

            return await query.ToPaginatedResultAsync(request);
        }

        public async Task<List<Announcement>> GetActiveBySiteIdAsync(long siteId)
        {
            return await _dbSet
                .Where(j => j.SiteId == siteId && j.IsActive)
                .ToListAsync();
        }
    }
}
