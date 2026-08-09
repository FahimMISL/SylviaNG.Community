using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// An employee report flagging a Listing for HR review (e.g. inappropriate content, scam).
/// </summary>
public class MarketplaceReport : Audit
{
    public long ReportId { get; set; }
    public long ListingId { get; set; }
    public long ReportedBy { get; set; }
    public string Reason { get; set; } = string.Empty;
    public new string Status { get; set; } = string.Empty;
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
