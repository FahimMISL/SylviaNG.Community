using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A trust-based, buyer-initiated record of "Employee X bought Quantity of Listing Y" -
/// there is no payment/checkout system anywhere in this app, so this is a simple confirmation,
/// not a transaction record. UnitPrice/Currency are snapshotted at purchase time so a later
/// price edit on the listing doesn't retroactively rewrite purchase history.
/// </summary>
public class Purchase : Audit
{
    public long PurchaseId { get; set; }
    public long ListingId { get; set; }
    public long BuyerId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
}
