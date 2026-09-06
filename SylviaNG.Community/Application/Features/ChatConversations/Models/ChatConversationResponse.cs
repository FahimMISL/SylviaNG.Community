using System.Text.Json.Serialization;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Application.Features.ChatConversations.Models
{
    public class ChatConversationResponse
    {
        public long ChatConversationId { get; set; }
        public ConversationTypeEnum Type { get; set; }
        public string? Title { get; set; }
        public long? GroupAvatarFileId { get; set; }
        public string? GroupAvatarUrl { get; set; }
        public long CreatedByEmployeeId { get; set; }
        public bool OnlyAdminsCanAddMembers { get; set; }
        [JsonConverter(typeof(NullableUtcDateTimeJsonConverter))]
        public DateTime? LastMessageAt { get; set; }
        public string? LastMessagePreview { get; set; }
        public List<ChatParticipantResponse> Participants { get; set; } = new();
    }
}
