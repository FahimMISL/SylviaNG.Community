using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportResolve
{
    public class MarketplaceReportResolveCommand : IRequest
    {
        public long ReportId { get; set; }
        public long ReviewerId { get; set; }
        public MarketplaceReportResolveRequest Request { get; set; }

        public MarketplaceReportResolveCommand(long reportId, long reviewerId, MarketplaceReportResolveRequest request)
        {
            ReportId = reportId;
            ReviewerId = reviewerId;
            Request = request;
        }
    }
}
