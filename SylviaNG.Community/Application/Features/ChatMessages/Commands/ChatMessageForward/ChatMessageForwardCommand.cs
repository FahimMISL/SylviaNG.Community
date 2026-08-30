using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageForward
{
    public class ChatMessageForwardCommand : IRequest<Unit>
    {
        public long ChatMessageId { get; set; }
        public ChatMessageForwardRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageForwardCommand(long chatMessageId, ChatMessageForwardRequest request, long callerEmployeeId)
        {
            ChatMessageId = chatMessageId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
