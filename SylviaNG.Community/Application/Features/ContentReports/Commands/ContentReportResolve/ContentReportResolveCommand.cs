using MediatR;
using SylviaNG.Community.Application.Features.ContentReports.Models;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportResolve
{
    public class ContentReportResolveCommand : IRequest
    {
        public long ReportId { get; set; }
        public ContentReportResolveRequest Request { get; set; }

        public ContentReportResolveCommand(long reportId, ContentReportResolveRequest request)
        {
            ReportId = reportId;
            Request = request;
        }
    }
}
