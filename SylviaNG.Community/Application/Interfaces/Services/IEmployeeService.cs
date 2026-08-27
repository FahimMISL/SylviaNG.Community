using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<long> CreateAsync(EmployeeCreateRequest request);

        Task UpdateProfileAsync(long employeeId, EmployeeUpdateProfileRequest request, long? viewerEmployeeId);

        Task UpdatePhotoAsync(long employeeId, string storagePath, long? viewerEmployeeId);

        Task UpdateCoverPhotoAsync(long employeeId, string storagePath, long? viewerEmployeeId);

        Task DeactivateAsync(long employeeId);

        Task<EmployeeResponse> GetByIdAsync(long employeeId, long? viewerEmployeeId, bool viewerIsHrAdmin);

        Task<PagedResult<EmployeeDirectoryCardResponse>> GetDirectoryPaginatedAsync(EmployeeFilterRequest request);

        Task<PagedResult<EmployeeManagementRowResponse>> GetManagementPaginatedAsync(EmployeeFilterRequest request);

        /// <summary>Feed sidebar "Today's Events" widget - active employees whose birthday or work anniversary falls on today's date.</summary>
        Task<List<TodayEventResponse>> GetTodayEventsAsync();

        /// <summary>Feed sidebar "New Joinees" widget - active employees who joined within the last 2 days.</summary>
        Task<List<NewJoineeResponse>> GetNewJoineesAsync();
    }
}
