using MediatR;
using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ActivityLogs.Queries.ActivityLogGetAllPaged
{
    public class ActivityLogGetAllPagedHandler : IRequestHandler<ActivityLogGetAllPagedQuery, PagedResult<ActivityLogResponse>>
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogGetAllPagedHandler(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        public async Task<PagedResult<ActivityLogResponse>> Handle(ActivityLogGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _activityLogService.GetPaginatedAsync(query.Request);
        }
    }
}
