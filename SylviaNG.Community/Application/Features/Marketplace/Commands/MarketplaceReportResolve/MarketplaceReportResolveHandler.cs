using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportResolve
{
    public class MarketplaceReportResolveHandler : IRequestHandler<MarketplaceReportResolveCommand>
    {
        private readonly IMarketplaceService _marketplaceService;

        public MarketplaceReportResolveHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task Handle(MarketplaceReportResolveCommand command, CancellationToken cancellationToken)
        {
            await _marketplaceService.ResolveReportAsync(command.ReportId, command.ReviewerId, command.Request);
        }
    }
}
