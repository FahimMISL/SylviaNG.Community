using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A comment on a Post. ParentCommentId is a nullable self-reference enabling
/// threaded replies (null = top-level comment).
/// </summary>
public class PostComment : Audit
{
    public long CommentId { get; set; }
    public long PostId { get; set; }
    public long EmployeeId { get; set; }
    public long? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
}
