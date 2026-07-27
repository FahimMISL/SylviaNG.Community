using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// One row per employee per notification category, controlling delivery channels.
/// Unique composite index on (EmployeeId, Category).
/// </summary>
public class NotificationPreference : Audit
{
    public long PreferenceId { get; set; }
    public long EmployeeId { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool InAppEnabled { get; set; } = true;
    public bool EmailEnabled { get; set; } = true;
}
