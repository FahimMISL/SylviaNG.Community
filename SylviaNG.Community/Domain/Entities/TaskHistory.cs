using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Insert-only audit log of meaningful changes made to a Task. Rows are written by
/// TaskUpdateHandler (via ITaskHistoryRepository.AddAsync) when it detects a change to
/// Status/Priority/AssignedTo/DueDate - there is no separate create/update/delete surface
/// for this entity.
/// </summary>
public class TaskHistory : Audit
{
    public long HistoryId { get; set; }
    public long TaskId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public long ChangedBy { get; set; }
}
