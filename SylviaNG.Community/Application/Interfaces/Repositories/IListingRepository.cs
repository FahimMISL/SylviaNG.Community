using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IListingRepository : IRepository<Listing>
    {
        Task<PagedResult<Listing>> GetPaginatedAsync(ListingFilterRequest request);
    }
}
