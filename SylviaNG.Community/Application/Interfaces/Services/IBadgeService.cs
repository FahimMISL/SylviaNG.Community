using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IBadgeService
    {
        Task<long> CreateAsync(BadgeCreateRequest request);
        Task DeleteAsync(long badgeId);
        Task<BadgeResponse> GetByIdAsync(long badgeId);
        Task<List<BadgeResponse>> GetAllAsync();
        Task<PagedResult<BadgeResponse>> GetPaginatedAsync(PagedRequest request);
        Task<long> AwardToEmployeeAsync(long employeeId, EmployeeBadgeAwardRequest request);
        Task<List<EmployeeBadgeResponse>> GetEmployeeBadgesAsync(long employeeId);
    }
}
