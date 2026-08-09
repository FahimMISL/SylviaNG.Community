using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class MarketplaceReportRepository : Repository<MarketplaceReport>, IMarketplaceReportRepository
    {
        public MarketplaceReportRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<MarketplaceReport>> GetPaginatedAsync(PagedRequest request)
        {
            var query = _dbSet.AsQueryable();

            request.SearchProperties ??= new[] { nameof(MarketplaceReport.Reason), nameof(MarketplaceReport.Status) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
