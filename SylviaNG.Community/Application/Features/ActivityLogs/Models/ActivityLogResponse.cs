namespace SylviaNG.Community.Application.Features.ActivityLogs.Models
{
    public class ActivityLogResponse
    {
        public long ActivityId { get; set; }
        public long EmployeeId { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
