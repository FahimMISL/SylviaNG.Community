using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingDelete
{
    public class ListingDeleteHandler : IRequestHandler<ListingDeleteCommand>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingDeleteHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task Handle(ListingDeleteCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.DeleteListingAsync(command.ListingId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
