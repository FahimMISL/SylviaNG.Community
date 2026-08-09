using SylviaNG.Community.Application.Features.EmployeeInterests.Models;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IInterestService
    {
        Task<long> CreateAsync(InterestCreateRequest request);
        Task DeleteAsync(long interestId);
        Task<InterestResponse> GetByIdAsync(long interestId);
        Task<List<InterestResponse>> GetAllAsync();
        Task<PagedResult<InterestResponse>> GetPaginatedAsync(PagedRequest request);
        Task<long> AssignToEmployeeAsync(long employeeId, EmployeeInterestAssignRequest request);
        Task RemoveFromEmployeeAsync(long employeeId, long interestId);
        Task<List<EmployeeInterestResponse>> GetEmployeeInterestsAsync(long employeeId);
    }
}
