using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReact
{
    public class ChatMessageReactCommand : IRequest<ChatMessageReactionResponse?>
    {
        public long ChatMessageId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageReactCommand(long chatMessageId, ReactionTypeEnum reactionType, long callerEmployeeId)
        {
            ChatMessageId = chatMessageId;
            ReactionType = reactionType;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
