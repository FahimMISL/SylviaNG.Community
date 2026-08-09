namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ListingUpdateRequest
    {
        public string? ListingType { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Condition { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}
