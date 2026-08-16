using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.PurchaseCreate
{
    public class PurchaseCreateHandler : IRequestHandler<PurchaseCreateCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public PurchaseCreateHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(PurchaseCreateCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.CreatePurchaseAsync(command.BuyerId, command.Request);
        }
    }
}
