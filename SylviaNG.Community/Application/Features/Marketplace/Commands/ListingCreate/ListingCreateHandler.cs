using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ListingCreate
{
    public class ListingCreateHandler : IRequestHandler<ListingCreateCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ListingCreateHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(ListingCreateCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.CreateListingAsync(command.SellerId, command.IsHrOrAdmin, command.Request);
        }
    }
}
