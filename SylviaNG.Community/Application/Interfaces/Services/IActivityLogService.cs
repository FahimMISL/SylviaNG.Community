using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IActivityLogService
    {
        /// <summary>
        /// Records an activity log entry. Called inline by other code that performs an
        /// action - there is no public "create a log entry" REST endpoint.
        /// </summary>
        Task<long> LogAsync(ActivityLogCreateRequest request);
        Task<PagedResult<ActivityLogResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
