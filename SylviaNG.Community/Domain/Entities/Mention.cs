using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Records that MentionedBy mentioned MentionedEmployeeId (via "@") inside some entity
/// (a Post or a PostComment, identified generically by EntityType/EntityId - no FK
/// constraint on the polymorphic pointer since it can target different tables).
/// </summary>
public class Mention : Audit
{
    public long MentionId { get; set; }
    public long MentionedEmployeeId { get; set; }
    public long MentionedBy { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
}
