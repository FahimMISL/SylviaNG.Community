using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A buyer's star rating + written review of a Listing they purchased. One review per
/// (ListingId, ReviewerId) - enforced at the DB layer too via a unique index.
/// </summary>
public class Review : Audit
{
    public long ReviewId { get; set; }
    public long ListingId { get; set; }
    public long ReviewerId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
