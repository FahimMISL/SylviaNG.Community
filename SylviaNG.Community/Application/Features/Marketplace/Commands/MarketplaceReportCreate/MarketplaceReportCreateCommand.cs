using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MarketplaceReportCreate
{
    public class MarketplaceReportCreateCommand : IRequest<long>
    {
        public long ReportedBy { get; set; }
        public MarketplaceReportCreateRequest Request { get; set; }

        public MarketplaceReportCreateCommand(long reportedBy, MarketplaceReportCreateRequest request)
        {
            ReportedBy = reportedBy;
            Request = request;
        }
    }
}
