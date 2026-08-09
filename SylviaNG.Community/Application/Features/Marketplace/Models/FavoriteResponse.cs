namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    public class FavoriteResponse
    {
        public long FavoriteId { get; set; }
        public long EmployeeId { get; set; }
        public long ListingId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
