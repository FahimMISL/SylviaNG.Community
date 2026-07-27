using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Insert-only audit log capturing before/after values of a data change on any table.
/// There is no public "create a log entry" REST endpoint - other code calls
/// IAuditLogService.LogAsync / IAuditLogRepository.AddAsync inline.
/// </summary>
public class AuditLog : Audit
{
    public long AuditId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public long RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public long PerformedBy { get; set; }
}
