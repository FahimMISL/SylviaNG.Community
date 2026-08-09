using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(long employeeId, long listingId)
        {
            return await _dbSet.AnyAsync(f => f.EmployeeId == employeeId && f.ListingId == listingId);
        }

        public async Task<Favorite?> GetAsync(long employeeId, long listingId)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.EmployeeId == employeeId && f.ListingId == listingId);
        }

        public async Task<List<Favorite>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _dbSet.Where(f => f.EmployeeId == employeeId).ToListAsync();
        }
    }
}
