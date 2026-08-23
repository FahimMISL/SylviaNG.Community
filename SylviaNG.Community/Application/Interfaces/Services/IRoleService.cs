using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<long> CreateAsync(RoleCreateRequest request);
        Task UpdateAsync(long roleId, RoleUpdateRequest request);
        Task DeleteAsync(long roleId);
        Task<RoleResponse> GetByIdAsync(long roleId);
        Task<PagedResult<RoleResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
