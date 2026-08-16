using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        Task<bool> ExistsForBuyerAndListingAsync(long buyerId, long listingId);
        Task<List<Purchase>> GetByBuyerIdAsync(long buyerId);
    }
}
