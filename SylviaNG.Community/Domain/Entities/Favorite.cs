using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Join entity between an Employee and a Listing they have bookmarked. Unique on
/// (EmployeeId, ListingId) - see FavoriteConfiguration.
/// </summary>
public class Favorite : Audit
{
    public long FavoriteId { get; set; }
    public long EmployeeId { get; set; }
    public long ListingId { get; set; }
}
