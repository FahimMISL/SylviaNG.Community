namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class PurchaseResponse
    {
        public long PurchaseId { get; set; }
        public long ListingId { get; set; }
        public long BuyerId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
