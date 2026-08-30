namespace SylviaNG.Community.Application.Features.ChatMessages.Models
{
    /// <summary>Lightweight quoted-snippet shown above a reply bubble - avoids the client needing a second fetch for the original message.</summary>
    public class ChatMessageReplyPreviewResponse
    {
        public long ChatMessageId { get; set; }
        public long SenderEmployeeId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? BodyPreview { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsDeleted { get; set; }
    }
}
