using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Reuses the existing ReactionTypeEnum already shared by PostReaction/RecognitionReaction/
/// CommentReaction. Unique (ChatMessageId, EmployeeId) - re-reacting overwrites. Wired up in
/// the interactions branch; the table ships now alongside the rest of the module's schema.
/// </summary>
public class ChatMessageReaction : Audit
{
    public long ChatMessageReactionId { get; set; }
    public long ChatMessageId { get; set; }
    public long EmployeeId { get; set; }
    public ReactionTypeEnum ReactionType { get; set; }
}
