using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ReviewGetAllForListing
{
    public class ReviewGetAllForListingHandler : IRequestHandler<ReviewGetAllForListingQuery, List<ReviewResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ReviewGetAllForListingHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<List<ReviewResponse>> Handle(ReviewGetAllForListingQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetReviewsForListingAsync(query.ListingId);
        }
    }
}
