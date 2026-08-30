using MediatR;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationMarkRead
{
    public class ChatConversationMarkReadCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationMarkReadCommand(long chatConversationId, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
