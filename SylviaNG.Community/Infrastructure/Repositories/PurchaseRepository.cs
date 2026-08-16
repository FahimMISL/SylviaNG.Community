using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsForBuyerAndListingAsync(long buyerId, long listingId)
        {
            return await _dbSet.AnyAsync(p => p.BuyerId == buyerId && p.ListingId == listingId);
        }

        public async Task<List<Purchase>> GetByBuyerIdAsync(long buyerId)
        {
            return await _dbSet.Where(p => p.BuyerId == buyerId).ToListAsync();
        }
    }
}
