using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.FavoriteAdd
{
    public class FavoriteAddHandler : IRequestHandler<FavoriteAddCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public FavoriteAddHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(FavoriteAddCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.AddFavoriteAsync(command.EmployeeId, command.Request);
        }
    }
}
