using MediatR;
using SylviaNG.Community.Application.Features.ChatReports.Models;

namespace SylviaNG.Community.Application.Features.ChatReports.Commands.ChatReportResolve
{
    public class ChatReportResolveCommand : IRequest
    {
        public long ReportId { get; set; }
        public ChatReportResolveRequest Request { get; set; }

        public ChatReportResolveCommand(long reportId, ChatReportResolveRequest request)
        {
            ReportId = reportId;
            Request = request;
        }
    }
}
