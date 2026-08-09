using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A unit of work assigned to an employee, optionally owned by a Team and optionally
/// generated from a RecurringTask. Named "Task" per the ERD - callers elsewhere in the
/// codebase that also need System.Threading.Tasks.Task should fully-qualify the BCL type
/// (or alias this entity) to avoid CS0104 ambiguity; this file itself has no async members
/// so no conflict arises here.
/// </summary>
public class Task : Audit
{
    public long TaskId { get; set; }
    public long TeamId { get; set; }
    public long AssignedBy { get; set; }
    public long AssignedTo { get; set; }
    public long? RecurringTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string TaskStatus { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int? ReminderDays { get; set; }
}
