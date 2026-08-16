using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<bool> ExistsAsync(long reviewerId, long listingId);
        Task<List<Review>> GetByListingIdAsync(long listingId);
        Task<(double? Average, int Count)> GetRatingSummaryAsync(long listingId);
        Task<Dictionary<long, (double? Average, int Count)>> GetRatingSummariesAsync(IEnumerable<long> listingIds);
    }
}
