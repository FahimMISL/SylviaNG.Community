namespace SylviaNG.Community.Application.Features.ActivityLogs.Models
{
    /// <summary>
    /// Input used by other code (not a public REST endpoint) to record an activity log entry
    /// via IActivityLogService.LogAsync.
    /// </summary>
    public class ActivityLogCreateRequest
    {
        public long EmployeeId { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
    }
}
