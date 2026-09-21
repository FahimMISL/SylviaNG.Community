using MediatR;
using SylviaNG.Community.Application.Features.ContentReports.Models;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportResolve
{
    public class ContentReportResolveCommand : IRequest
    {
        public long ReportId { get; set; }
        public long? ReviewerId { get; set; }
        public ContentReportResolveRequest Request { get; set; }

        public ContentReportResolveCommand(long reportId, long? reviewerId, ContentReportResolveRequest request)
        {
            ReportId = reportId;
            ReviewerId = reviewerId;
            Request = request;
        }
    }
}
