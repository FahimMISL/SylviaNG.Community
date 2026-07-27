namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class MarketplaceReportResponse
    {
        public long ReportId { get; set; }
        public long ListingId { get; set; }
        public long ReportedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
