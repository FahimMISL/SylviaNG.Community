using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Catalog of badges that can be awarded to employees, typically by HR.
/// </summary>
public class Badge : Audit
{
    public long BadgeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }
}
