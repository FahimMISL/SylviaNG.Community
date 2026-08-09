using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteRemove
{
    public class FavoriteRemoveHandler : IRequestHandler<FavoriteRemoveCommand>
    {
        private readonly IMarketplaceService _marketplaceService;

        public FavoriteRemoveHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task Handle(FavoriteRemoveCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.RemoveFavoriteAsync(command.EmployeeId, command.ListingId);
        }
    }
}
