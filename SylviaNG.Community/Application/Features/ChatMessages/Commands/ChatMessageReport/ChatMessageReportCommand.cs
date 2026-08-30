using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReport
{
    public class ChatMessageReportCommand : IRequest<Unit>
    {
        public long ChatMessageId { get; set; }
        public ChatMessageReportRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageReportCommand(long chatMessageId, ChatMessageReportRequest request, long callerEmployeeId)
        {
            ChatMessageId = chatMessageId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
