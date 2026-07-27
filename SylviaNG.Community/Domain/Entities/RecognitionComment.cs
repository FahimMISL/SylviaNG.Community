using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A comment on a Recognition post. Supports threaded replies via ParentCommentId.
/// </summary>
public class RecognitionComment : Audit
{
    public long CommentId { get; set; }
    public long RecognitionId { get; set; }
    public long EmployeeId { get; set; }
    public long? ParentCommentId { get; set; }
    public string Comment { get; set; } = string.Empty;
}
