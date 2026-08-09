using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class CommentReaction : Audit
{
    public long ReactionId { get; set; }
    public long CommentId { get; set; }
    public long EmployeeId { get; set; }
    public ReactionTypeEnum ReactionType { get; set; }
}
