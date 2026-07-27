using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingUpdate
{
    public class ListingUpdateHandler : IRequestHandler<ListingUpdateCommand>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingUpdateHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task Handle(ListingUpdateCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.UpdateListingAsync(command.ListingId, command.CallerEmployeeId, command.IsHrOrAdmin, command.Request);
        }
    }
}
