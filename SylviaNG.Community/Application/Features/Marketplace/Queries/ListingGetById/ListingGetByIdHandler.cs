using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ListingGetById
{
    public class ListingGetByIdHandler : IRequestHandler<ListingGetByIdQuery, ListingResponse>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingGetByIdHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<ListingResponse> Handle(ListingGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetListingByIdAsync(query.ListingId);
        }
    }
}
