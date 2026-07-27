using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ListingImageGetAll
{
    public class ListingImageGetAllHandler : IRequestHandler<ListingImageGetAllQuery, List<ListingImageResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingImageGetAllHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<List<ListingImageResponse>> Handle(ListingImageGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetImagesAsync(query.ListingId);
        }
    }
}
