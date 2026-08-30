using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A moderation report filed against a Messenger conversation or a specific message within
/// it. Deliberately a new entity rather than a generalized ContentReport - this codebase
/// already has two independent, non-polymorphic report entities (ContentReport for Posts,
/// MarketplaceReport for Listings), so a third follows the established convention rather
/// than reworking a shipped one. ChatMessageId is nullable: report one specific message, or
/// the conversation generally (e.g. a pattern of harassment). Wired up in the moderation
/// branch; the table ships now alongside the rest of the module's schema.
/// </summary>
public class ChatReport : Audit
{
    public long ChatReportId { get; set; }
    public long ReportedByEmployeeId { get; set; }
    public long ChatConversationId { get; set; }
    public long? ChatMessageId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public new string Status { get; set; } = string.Empty;
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
