using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IDesignationService
    {
        Task<long> CreateAsync(DesignationCreateRequest request);
        Task UpdateAsync(long designationId, DesignationUpdateRequest request);
        Task DeleteAsync(long designationId);
        Task<DesignationResponse> GetByIdAsync(long designationId);
        Task<PagedResult<DesignationResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
