namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class MessageResponse
    {
        public long MessageId { get; set; }
        public long ConversationId { get; set; }
        public long SenderId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}
