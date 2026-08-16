using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long reviewerId, long listingId)
        {
            return await _dbSet.AnyAsync(r => r.ReviewerId == reviewerId && r.ListingId == listingId);
        }

        public async Task<List<Review>> GetByListingIdAsync(long listingId)
        {
            return await _dbSet.Where(r => r.ListingId == listingId).ToListAsync();
        }

        public async Task<(double? Average, int Count)> GetRatingSummaryAsync(long listingId)
        {
            var ratings = await _dbSet.Where(r => r.ListingId == listingId).Select(r => r.Rating).ToListAsync();
            return (ratings.Count > 0 ? ratings.Average() : (double?)null, ratings.Count);
        }

        public async Task<Dictionary<long, (double? Average, int Count)>> GetRatingSummariesAsync(IEnumerable<long> listingIds)
        {
            var grouped = await _dbSet.Where(r => listingIds.Contains(r.ListingId))
                .GroupBy(r => r.ListingId)
                .Select(g => new { ListingId = g.Key, Average = g.Average(r => (double)r.Rating), Count = g.Count() })
                .ToListAsync();

            return grouped.ToDictionary(g => g.ListingId, g => ((double?)g.Average, g.Count));
        }
    }
}
