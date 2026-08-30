using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// One row per employee per conversation. IsAdmin is per-participant (supports co-admins
/// in a group, avoids an orphaned-admin problem if the sole admin leaves). Removed/left
/// members are soft-removed (LeftAt set, row kept) to preserve message attribution; a
/// re-added employee has their existing row's LeftAt cleared rather than a duplicate
/// inserted. LastReadAt is a single watermark (not a per-message receipt table) - it answers
/// every read-receipt/unread-count need with one UPDATE instead of one row per message read.
/// </summary>
public class ChatParticipant : Audit
{
    public long ChatParticipantId { get; set; }
    public long ChatConversationId { get; set; }
    public long EmployeeId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public DateTime? LastReadAt { get; set; }
    public bool IsMuted { get; set; }
    public bool IsPinned { get; set; }
    public DateTime? PinnedAt { get; set; }
}
