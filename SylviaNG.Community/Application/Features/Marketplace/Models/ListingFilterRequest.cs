using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Models
{
    /// <summary>
    /// Paging/filter request for browsing listings - category/status/approval/price-range
    /// filters plus the inherited free-text search over Title/Description.
    /// </summary>
    public class ListingFilterRequest : PagedRequest
    {
        public string? Category { get; set; }
        public string? Status { get; set; }
        public string? ApprovalStatus { get; set; }
        public long? SellerId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
