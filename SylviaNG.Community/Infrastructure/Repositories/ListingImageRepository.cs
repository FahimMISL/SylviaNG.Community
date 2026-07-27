using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ListingImageRepository : Repository<ListingImage>, IListingImageRepository
    {
        public ListingImageRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ListingImage>> GetByListingIdAsync(long listingId)
        {
            return await _dbSet
                .Where(li => li.ListingId == listingId)
                .OrderBy(li => li.DisplayOrder)
                .ToListAsync();
        }
    }
}
