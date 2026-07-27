using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Insert-only activity log: a row is written by whatever code performs an action.
/// There is no public "create a log entry" REST endpoint - other code calls
/// IActivityLogService.LogAsync / IActivityLogRepository.AddAsync inline.
/// EntityId is a generic, non-FK polymorphic pointer.
/// </summary>
public class ActivityLog : Audit
{
    public long ActivityId { get; set; }
    public long EmployeeId { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
}
