using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ReviewImageGetAll
{
    public class ReviewImageGetAllHandler : IRequestHandler<ReviewImageGetAllQuery, List<ReviewImageResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ReviewImageGetAllHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<List<ReviewImageResponse>> Handle(ReviewImageGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetReviewImagesAsync(query.ReviewId);
        }
    }
}
