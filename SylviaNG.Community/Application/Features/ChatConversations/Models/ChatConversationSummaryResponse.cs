using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.ChatConversations.Models
{
    /// <summary>
    /// One row of the Messenger inbox list. DisplayName/OtherEmployeeId resolve to the
    /// other participant for Direct conversations, or the group's own Title for Group.
    /// UnreadCount/IsMuted/IsPinned are all relative to whichever employee requested it.
    /// </summary>
    public class ChatConversationSummaryResponse
    {
        public long ChatConversationId { get; set; }
        public ConversationTypeEnum Type { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public long? OtherEmployeeId { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string? LastMessagePreview { get; set; }
        public int UnreadCount { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
    }
}
