using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<long> CreateAsync(DepartmentCreateRequest request);
        Task UpdateAsync(long departmentId, DepartmentUpdateRequest request);
        Task DeleteAsync(long departmentId);
        Task<DepartmentResponse> GetByIdAsync(long departmentId);
        Task<PagedResult<DepartmentResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
