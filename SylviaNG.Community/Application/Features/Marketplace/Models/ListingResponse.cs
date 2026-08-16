namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ListingResponse
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
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
