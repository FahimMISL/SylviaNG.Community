using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    public class ChatMessageAttachmentResponse
    {
        public long ChatMessageAttachmentId { get; set; }
        public long FileStorageId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public long FileSize { get; set; }
        public ChatAttachmentTypeEnum AttachmentType { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
