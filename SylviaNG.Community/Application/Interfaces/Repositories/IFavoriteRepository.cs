using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IFavoriteRepository : IRepository<Favorite>
    {
        Task<bool> ExistsAsync(long employeeId, long listingId);
        Task<Favorite?> GetAsync(long employeeId, long listingId);
        Task<List<Favorite>> GetByEmployeeIdAsync(long employeeId);
    }
}
