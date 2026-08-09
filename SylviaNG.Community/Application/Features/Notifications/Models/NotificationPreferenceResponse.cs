namespace SylviaNG.Community.Application.Features.Notifications.Models
{
    public class NotificationPreferenceResponse
    {
        public long PreferenceId { get; set; }
        public long EmployeeId { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool InAppEnabled { get; set; }
        public bool EmailEnabled { get; set; }
    }
}
