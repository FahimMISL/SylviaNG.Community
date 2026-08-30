using MediatR;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetMuted
{
    public class ChatConversationSetMutedCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsMuted { get; set; }

        public ChatConversationSetMutedCommand(long chatConversationId, long callerEmployeeId, bool isMuted)
        {
            ChatConversationId = chatConversationId;
            CallerEmployeeId = callerEmployeeId;
            IsMuted = isMuted;
        }
    }
}
