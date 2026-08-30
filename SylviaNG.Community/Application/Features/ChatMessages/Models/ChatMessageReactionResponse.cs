using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    public class ChatMessageReactionResponse
    {
        public long ChatMessageReactionId { get; set; }
        public long ChatConversationId { get; set; }
        public long ChatMessageId { get; set; }
        public long EmployeeId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
    }
}
