using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<long> CreateAsync(EmployeeCreateRequest request);

        Task UpdateProfileAsync(long employeeId, EmployeeUpdateProfileRequest request, long? viewerEmployeeId);

        Task DeactivateAsync(long employeeId);

        Task<EmployeeResponse> GetByIdAsync(long employeeId, long? viewerEmployeeId, bool viewerIsHrAdmin);

        Task<PagedResult<EmployeeDirectoryCardResponse>> GetDirectoryPaginatedAsync(EmployeeFilterRequest request);

        Task<PagedResult<EmployeeManagementRowResponse>> GetManagementPaginatedAsync(EmployeeFilterRequest request);
    }
}
