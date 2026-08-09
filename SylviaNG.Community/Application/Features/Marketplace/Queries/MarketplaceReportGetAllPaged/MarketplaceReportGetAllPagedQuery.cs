using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.MarketplaceReportGetAllPaged
{
    public class MarketplaceReportGetAllPagedQuery : IRequest<PagedResult<MarketplaceReportResponse>>
    {
        public PagedRequest Request { get; set; }

        public MarketplaceReportGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
