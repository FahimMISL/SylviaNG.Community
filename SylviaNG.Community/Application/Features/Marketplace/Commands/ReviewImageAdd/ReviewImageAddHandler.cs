using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewImageAdd
{
    public class ReviewImageAddHandler : IRequestHandler<ReviewImageAddCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ReviewImageAddHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(ReviewImageAddCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.AddReviewImageAsync(command.ReviewId, command.Request);
        }
    }
}
