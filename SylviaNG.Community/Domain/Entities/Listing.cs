using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A marketplace item or service posted by an employee (the seller). Goes through
/// an HR approval workflow (ApprovalStatus) before becoming publicly visible.
/// </summary>
public class Listing : Audit
{
    public long ListingId { get; set; }
    public long SellerId { get; set; }
    public string ListingType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Condition { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Location { get; set; }
    public new string Status { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
    public long? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
}
