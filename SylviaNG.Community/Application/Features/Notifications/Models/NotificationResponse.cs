namespace SylviaNG.Community.Application.Features.Notifications.Models
{
    public class NotificationResponse
    {
        public long NotificationId { get; set; }
        public long EmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? Category { get; set; }
        public string? RelatedEntityType { get; set; }
        public long? RelatedEntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
