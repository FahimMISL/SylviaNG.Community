using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A notification delivered to an employee (e.g. announcement published, team invite, etc.).
/// RelatedEntityType/RelatedEntityId form a generic, non-FK polymorphic pointer back to the
/// entity that triggered the notification.
/// </summary>
public class Notification : Audit
{
    public long NotificationId { get; set; }
    public long EmployeeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? Category { get; set; }
    public string? RelatedEntityType { get; set; }
    public long? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
