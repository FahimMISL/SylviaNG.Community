namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ListingCreateRequest
    {
        public string ListingType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Condition { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Location { get; set; }
        public int Quantity { get; set; } = 1;

        /// <summary>Employee-only: save privately instead of submitting for HR/Admin review. Ignored for HR/Admin callers, who always publish immediately.</summary>
        public bool SaveAsDraft { get; set; }
    }
}
