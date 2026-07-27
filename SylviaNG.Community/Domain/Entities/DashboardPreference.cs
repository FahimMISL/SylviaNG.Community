using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// One row per employee per dashboard widget, controlling order/visibility of "my dashboard".
/// Unique composite index on (EmployeeId, WidgetName) - an employee can have many widgets.
/// </summary>
public class DashboardPreference : Audit
{
    public long PreferenceId { get; set; }
    public long EmployeeId { get; set; }
    public string WidgetName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public DateTime LastModified { get; set; }
}
