using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IListingImageRepository : IRepository<ListingImage>
    {
        Task<List<ListingImage>> GetByListingIdAsync(long listingId);
    }
}
