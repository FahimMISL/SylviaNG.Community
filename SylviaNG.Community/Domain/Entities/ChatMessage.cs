using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A single Messenger message. SentAt duplicates Audit.CreatedAt deliberately (that one is
/// nullable/interceptor-populated) so message ordering has a guaranteed non-null sort key at
/// insert time - same duplication already accepted on Marketplace's own Message entity.
/// SharedContentType/SharedContentId form a polymorphic, non-FK pointer (Post/Listing/Event)
/// for US-12.15, mirroring FileStorage.EntityId's already-accepted pattern in this codebase.
/// MessageType.System covers inline "X added Y"/"X left" audit-trail lines in the thread.
/// </summary>
public class ChatMessage : Audit
{
    public long ChatMessageId { get; set; }
    public long ChatConversationId { get; set; }
    public long SenderEmployeeId { get; set; }
    public string? Body { get; set; }
    public MessageTypeEnum MessageType { get; set; } = MessageTypeEnum.Text;
    public SharedContentTypeEnum? SharedContentType { get; set; }
    public long? SharedContentId { get; set; }
    public DateTime SentAt { get; set; }

    /// <summary>"Remove for Everyone" tombstone - deliberately NOT Audit.DeletedAt, since that's
    /// globally query-filtered out and would make the message vanish rather than render as a
    /// removed-message placeholder the way Messenger does. Body/attachments are blanked out of
    /// the response once this is true, but the row (and its history) is kept.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>Self-referencing, same conversation only (enforced in ChatMessageService.SendAsync) - the message being replied to.</summary>
    public long? ReplyToMessageId { get; set; }

    /// <summary>Set when this message was created via ChatMessageService.ForwardAsync rather than sent directly.</summary>
    public bool IsForwarded { get; set; }
}
