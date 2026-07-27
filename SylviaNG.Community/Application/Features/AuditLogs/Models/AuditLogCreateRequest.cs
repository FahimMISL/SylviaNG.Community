namespace SylviaNG.Community.Application.Features.AuditLogs.Models
{
    /// <summary>
    /// Input used by other code (not a public REST endpoint) to record an audit log entry
    /// via IAuditLogService.LogAsync.
    /// </summary>
    public class AuditLogCreateRequest
    {
        public string TableName { get; set; } = string.Empty;
        public long RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public long PerformedBy { get; set; }
    }
}
