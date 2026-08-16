using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.PurchaseGetAllForEmployee
{
    public class PurchaseGetAllForEmployeeHandler : IRequestHandler<PurchaseGetAllForEmployeeQuery, List<PurchaseResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public PurchaseGetAllForEmployeeHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<List<PurchaseResponse>> Handle(PurchaseGetAllForEmployeeQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetPurchasesForEmployeeAsync(query.EmployeeId);
        }
    }
}
