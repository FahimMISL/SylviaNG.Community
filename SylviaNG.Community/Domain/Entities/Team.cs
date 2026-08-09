using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Who created the team is tracked via the inherited Audit.CreatedBy - no separate field needed.
/// </summary>
public class Team : Audit
{
    public long TeamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? SupervisorId { get; set; }
    public bool IsActive { get; set; } = true;
}
