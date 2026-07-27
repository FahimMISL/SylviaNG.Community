using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A social feed post authored by an employee. Type/Visibility/ReactionType-style
/// fields are kept as plain strings (no enums) per module convention.
/// </summary>
public class Post : Audit
{
    public long PostId { get; set; }
    public long EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string? Content { get; set; }
    public bool IsAnnouncement { get; set; }
    public bool IsPoll { get; set; }
    public bool IsLocked { get; set; }
    public bool IsHidden { get; set; }
}
