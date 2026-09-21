using MediatR;
using SylviaNG.Community.Application.Features.ChatReports.Models;

namespace SylviaNG.Community.Application.Features.ChatReports.Commands.ChatReportResolve
{
    public class ChatReportResolveCommand : IRequest
    {
        public long ReportId { get; set; }
        public long? ReviewerId { get; set; }
        public ChatReportResolveRequest Request { get; set; }

        public ChatReportResolveCommand(long reportId, long? reviewerId, ChatReportResolveRequest request)
        {
            ReportId = reportId;
            ReviewerId = reviewerId;
            Request = request;
        }
    }
}
