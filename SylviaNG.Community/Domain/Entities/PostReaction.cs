using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class PostReaction : Audit
{
    public long ReactionId { get; set; }
    public long PostId { get; set; }
    public long EmployeeId { get; set; }
    public string ReactionType { get; set; } = string.Empty;
}
