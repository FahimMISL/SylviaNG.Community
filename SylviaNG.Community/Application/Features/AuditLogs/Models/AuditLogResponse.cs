namespace SylviaNG.Community.Application.Features.AuditLogs.Models
{
    public class AuditLogResponse
    {
        public long AuditId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public long RecordId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public long PerformedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
