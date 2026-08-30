using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatConversations.Models
{
    public class ChatConversationCreateRequest
    {
        public ConversationTypeEnum Type { get; set; } = ConversationTypeEnum.Direct;
        public string? Title { get; set; }
        public List<long> ParticipantEmployeeIds { get; set; } = new();
    }
}
