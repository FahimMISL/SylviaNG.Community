namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ReviewImageResponse
    {
        public long ImageId { get; set; }
        public long ReviewId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
