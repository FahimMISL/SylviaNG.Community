namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ReviewCreateRequest
    {
        public long ListingId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
