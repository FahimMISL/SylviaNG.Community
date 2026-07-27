using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A moderation report filed against a Post. ReviewedBy/ReviewedAt are populated
/// when an HR/Admin resolves the report.
/// </summary>
public class ContentReport : Audit
{
    public long ReportId { get; set; }
    public long ReportedBy { get; set; }
    public long PostId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public new string Status { get; set; } = string.Empty;
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
