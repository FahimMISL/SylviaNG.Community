namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class MarketplaceReportCreateRequest
    {
        public long ListingId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
