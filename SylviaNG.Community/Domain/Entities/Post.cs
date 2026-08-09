using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A social feed post authored by an employee. Type stays a free-form string (no
/// fixed value set - IsAnnouncement/IsPoll are the actual type-distinguishing flags);
/// Visibility is a closed set, so it's an enum (stored as string via EF conversion).
/// </summary>
public class Post : Audit
{
    public long PostId { get; set; }
    public long EmployeeId { get; set; }

    /// <summary>
    /// Null for ordinary feed posts. When set, this post belongs to a Group; whether it also
    /// surfaces on the main company feed is decided at query time from the Group's own
    /// Visibility (see PostRepository.GetFeedPaginatedAsync), not cached here.
    /// </summary>
    public long? GroupId { get; set; }

    public string Type { get; set; } = string.Empty;
    public VisibilityEnum Visibility { get; set; } = VisibilityEnum.Everyone;
    public string? Content { get; set; }
    public bool IsAnnouncement { get; set; }
    public bool IsPoll { get; set; }
    public bool IsLocked { get; set; }
    public bool IsHidden { get; set; }
}
