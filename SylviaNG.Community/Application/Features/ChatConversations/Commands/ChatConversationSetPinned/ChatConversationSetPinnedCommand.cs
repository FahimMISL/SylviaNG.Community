using MediatR;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetPinned
{
    public class ChatConversationSetPinnedCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsPinned { get; set; }

        public ChatConversationSetPinnedCommand(long chatConversationId, long callerEmployeeId, bool isPinned)
        {
            ChatConversationId = chatConversationId;
            CallerEmployeeId = callerEmployeeId;
            IsPinned = isPinned;
        }
    }
}
