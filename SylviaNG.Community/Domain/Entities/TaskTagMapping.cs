using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Join entity between Task and TaskTag. Unique composite index on (TaskId, TagId)
/// enforced in TaskTagMappingConfiguration.
/// </summary>
public class TaskTagMapping : Audit
{
    public long MappingId { get; set; }
    public long TaskId { get; set; }
    public long TagId { get; set; }
}
