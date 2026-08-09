using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A buyer/seller conversation thread scoped to a single Listing. Plain REST messaging -
/// no real-time/SignalR transport.
/// </summary>
public class Conversation : Audit
{
    public long ConversationId { get; set; }
    public long ListingId { get; set; }
    public new string Status { get; set; } = string.Empty;
}
