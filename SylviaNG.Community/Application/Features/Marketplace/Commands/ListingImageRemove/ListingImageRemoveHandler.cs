using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingImageRemove
{
    public class ListingImageRemoveHandler : IRequestHandler<ListingImageRemoveCommand>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingImageRemoveHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task Handle(ListingImageRemoveCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.RemoveImageAsync(command.ListingId, command.ImageId);
        }
    }
}
