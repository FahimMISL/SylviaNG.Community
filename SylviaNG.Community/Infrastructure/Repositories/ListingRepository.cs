using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ListingRepository : Repository<Listing>, IListingRepository
    {
        public ListingRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Listing>> GetPaginatedAsync(ListingFilterRequest request)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(l => l.Category == request.Category);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(l => l.Status == request.Status);
            else
                query = query.Where(l => l.Status != "Removed");

            if (!string.IsNullOrEmpty(request.ApprovalStatus))
                query = query.Where(l => l.ApprovalStatus == request.ApprovalStatus);

            if (request.SellerId.HasValue)
                query = query.Where(l => l.SellerId == request.SellerId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(l => l.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(l => l.Price <= request.MaxPrice.Value);

            request.SearchProperties ??= new[] { nameof(Listing.Title), nameof(Listing.Description) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
