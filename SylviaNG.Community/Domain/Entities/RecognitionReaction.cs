using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A reaction (e.g. like, clap) from an employee on a Recognition post.
/// </summary>
public class RecognitionReaction : Audit
{
    public long ReactionId { get; set; }
    public long RecognitionId { get; set; }
    public long EmployeeId { get; set; }
    public string ReactionType { get; set; } = string.Empty;
}
