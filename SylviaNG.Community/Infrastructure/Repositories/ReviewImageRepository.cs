using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ReviewImageRepository : Repository<ReviewImage>, IReviewImageRepository
    {
        public ReviewImageRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ReviewImage>> GetByReviewIdAsync(long reviewId)
        {
            return await _dbSet
                .Where(ri => ri.ReviewId == reviewId)
                .OrderBy(ri => ri.DisplayOrder)
                .ToListAsync();
        }
    }
}
