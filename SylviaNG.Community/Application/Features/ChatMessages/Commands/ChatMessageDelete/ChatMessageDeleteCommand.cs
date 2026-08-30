using MediatR;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageDelete
{
    public class ChatMessageDeleteCommand : IRequest<Unit>
    {
        public long ChatMessageId { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageDeleteCommand(long chatMessageId, long callerEmployeeId)
        {
            ChatMessageId = chatMessageId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
