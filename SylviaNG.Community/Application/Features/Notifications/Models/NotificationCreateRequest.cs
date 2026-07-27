namespace SylviaNG.Community.Application.Features.Notifications.Models
{
    public class NotificationCreateRequest
    {
        public long EmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? Category { get; set; }
        public string? RelatedEntityType { get; set; }
        public long? RelatedEntityId { get; set; }
    }
}
