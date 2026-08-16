using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.PurchaseHasPurchased
{
    public class PurchaseHasPurchasedHandler : IRequestHandler<PurchaseHasPurchasedQuery, bool>
    {
        private readonly IMarketplaceService _marketplaceService;

        public PurchaseHasPurchasedHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<bool> Handle(PurchaseHasPurchasedQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.HasPurchasedAsync(query.EmployeeId, query.ListingId);
        }
    }
}
