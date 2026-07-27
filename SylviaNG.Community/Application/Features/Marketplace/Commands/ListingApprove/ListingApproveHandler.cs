using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingApprove
{
    public class ListingApproveHandler : IRequestHandler<ListingApproveCommand, Unit>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingApproveHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<Unit> Handle(ListingApproveCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.ApproveListingAsync(command.ListingId, command.ApproverId);
            return Unit.Value;
        }
    }
}
