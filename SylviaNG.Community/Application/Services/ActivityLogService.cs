using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    /// <summary>
    /// ActivityLog is an insert-only log table: LogAsync is called inline by other code
    /// that performs an action (there is no public "create a log entry" REST endpoint).
    /// </summary>
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivityLogService(IActivityLogRepository activityLogRepository, IUnitOfWork unitOfWork)
        {
            _activityLogRepository = activityLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> LogAsync(ActivityLogCreateRequest request)
        {
            var entity = request.ToEntity();
            await _activityLogRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ActivityId;
        }

        public async Task<PagedResult<ActivityLogResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _activityLogRepository.GetPaginatedAsync(request);

            return new PagedResult<ActivityLogResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
