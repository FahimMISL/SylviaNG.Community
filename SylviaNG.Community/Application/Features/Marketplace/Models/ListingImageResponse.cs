namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ListingImageResponse
    {
        public long ImageId { get; set; }
        public long ListingId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
