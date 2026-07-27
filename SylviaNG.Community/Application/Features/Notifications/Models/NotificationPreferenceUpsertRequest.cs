namespace SylviaNG.Community.Application.Features.Notifications.Models
{
    public class NotificationPreferenceUpsertRequest
    {
        public long EmployeeId { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool InAppEnabled { get; set; } = true;
        public bool EmailEnabled { get; set; } = true;
    }
}
