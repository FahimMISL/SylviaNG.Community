using MediatR;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageSetPinned
{
    public class ChatMessageSetPinnedCommand : IRequest<Unit>
    {
        public long ChatMessageId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsPinned { get; set; }

        public ChatMessageSetPinnedCommand(long chatMessageId, long callerEmployeeId, bool isPinned)
        {
            ChatMessageId = chatMessageId;
            CallerEmployeeId = callerEmployeeId;
            IsPinned = isPinned;
        }
    }
}
