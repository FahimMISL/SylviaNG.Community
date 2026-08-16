namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class PurchaseCreateRequest
    {
        public long ListingId { get; set; }
        public int Quantity { get; set; }
    }
}
