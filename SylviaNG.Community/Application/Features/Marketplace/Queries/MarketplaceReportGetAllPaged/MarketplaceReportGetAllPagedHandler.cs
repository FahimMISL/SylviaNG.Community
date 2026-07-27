using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.MarketplaceReportGetAllPaged
{
    public class MarketplaceReportGetAllPagedHandler : IRequestHandler<MarketplaceReportGetAllPagedQuery, PagedResult<MarketplaceReportResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public MarketplaceReportGetAllPagedHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<PagedResult<MarketplaceReportResponse>> Handle(MarketplaceReportGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetReportsPagedAsync(query.Request);
        }
    }
}
