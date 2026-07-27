using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageAdd
{
    public class ListingImageAddHandler : IRequestHandler<ListingImageAddCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingImageAddHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(ListingImageAddCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.AddImageAsync(command.ListingId, command.Request);
        }
    }
}
