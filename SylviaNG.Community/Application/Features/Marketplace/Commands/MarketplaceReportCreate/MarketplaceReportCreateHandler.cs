using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportCreate
{
    public class MarketplaceReportCreateHandler : IRequestHandler<MarketplaceReportCreateCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public MarketplaceReportCreateHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(MarketplaceReportCreateCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.CreateReportAsync(command.ReportedBy, command.Request);
        }
    }
}
