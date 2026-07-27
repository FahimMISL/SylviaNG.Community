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
    }
}
