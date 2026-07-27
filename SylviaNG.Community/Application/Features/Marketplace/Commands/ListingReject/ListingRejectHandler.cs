using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingReject
{
    public class ListingRejectHandler : IRequestHandler<ListingRejectCommand, Unit>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingRejectHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<Unit> Handle(ListingRejectCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.RejectListingAsync(command.ListingId, command.ApproverId, command.Request);
            return Unit.Value;
        }
    }
}
