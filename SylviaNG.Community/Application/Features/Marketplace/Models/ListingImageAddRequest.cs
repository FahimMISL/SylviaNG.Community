namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class ListingImageAddRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public long? FileStorageId { get; set; }
    }
}
