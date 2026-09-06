using System.Text.Json.Serialization;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    public class ChatMessageResponse
    {
        public long ChatMessageId { get; set; }
        public long ChatConversationId { get; set; }
        public long SenderEmployeeId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderPhotoUrl { get; set; }
        public string? Body { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public SharedContentTypeEnum? SharedContentType { get; set; }
        public long? SharedContentId { get; set; }
        [JsonConverter(typeof(UtcDateTimeJsonConverter))]
        public DateTime SentAt { get; set; }
        public List<ChatMessageAttachmentResponse> Attachments { get; set; } = new();
        public List<ChatMessageReactionResponse> Reactions { get; set; } = new();
        public bool IsDeleted { get; set; }
        public bool IsForwarded { get; set; }
        public ChatMessageReplyPreviewResponse? ReplyTo { get; set; }
        public bool IsPinned { get; set; }
        [JsonConverter(typeof(NullableUtcDateTimeJsonConverter))]
        public DateTime? PinnedAt { get; set; }
        public long? PinnedByEmployeeId { get; set; }
    }
}
