using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewCreate
{
    public class ReviewCreateHandler : IRequestHandler<ReviewCreateCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ReviewCreateHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(ReviewCreateCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.CreateReviewAsync(command.ReviewerId, command.Request);
        }
    }
}
