using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageSend
{
    public class ChatMessageSendCommand : IRequest<ChatMessageResponse>
    {
        public long ChatConversationId { get; set; }
        public ChatMessageSendRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageSendCommand(long chatConversationId, ChatMessageSendRequest request, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
