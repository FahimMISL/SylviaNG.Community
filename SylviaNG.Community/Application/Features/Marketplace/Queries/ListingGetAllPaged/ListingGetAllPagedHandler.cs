using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetAllPaged
{
    public class ListingGetAllPagedHandler : IRequestHandler<ListingGetAllPagedQuery, PagedResult<ListingResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingGetAllPagedHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<PagedResult<ListingResponse>> Handle(ListingGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetListingsPagedAsync(query.Request);
        }
    }
}
