using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IBranchService
    {
        Task<long> CreateAsync(BranchCreateRequest request);
        Task UpdateAsync(long branchId, BranchUpdateRequest request);
        Task DeleteAsync(long branchId);
        Task<BranchResponse> GetByIdAsync(long branchId);
        Task<PagedResult<BranchResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
