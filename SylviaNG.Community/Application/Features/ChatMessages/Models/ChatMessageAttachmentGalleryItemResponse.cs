using System.Text.Json.Serialization;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Utils;

namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    /// <summary>One row of the "Media and Files" gallery - a ChatMessageAttachmentResponse annotated with
    /// sender/timing so the gallery doesn't need a second round trip per item.</summary>
    public class ChatMessageAttachmentGalleryItemResponse
    {
        public long ChatMessageAttachmentId { get; set; }
        public long ChatMessageId { get; set; }
        public long FileStorageId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public long FileSize { get; set; }
        public ChatAttachmentTypeEnum AttachmentType { get; set; }
        public int? DurationSeconds { get; set; }
        public long SenderEmployeeId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        [JsonConverter(typeof(UtcDateTimeJsonConverter))]
        public DateTime SentAt { get; set; }
    }
}
