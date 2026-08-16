namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ReviewResponse
    {
        public long ReviewId { get; set; }
        public long ListingId { get; set; }
        public long ReviewerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
