using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Defines a recurrence pattern (frequency/interval/date range) that a Task can optionally
/// be generated from. Recurrence generation itself is out of scope here - this entity only
/// stores the recurrence definition that Task.RecurringTaskId points back to.
/// </summary>
public class RecurringTask : Audit
{
    public long RecurringTaskId { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public int IntervalValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
