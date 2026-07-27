using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.FavoriteGetAll
{
    public class FavoriteGetAllHandler : IRequestHandler<FavoriteGetAllQuery, List<FavoriteResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public FavoriteGetAllHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<List<FavoriteResponse>> Handle(FavoriteGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetFavoritesAsync(query.EmployeeId);
        }
    }
}
