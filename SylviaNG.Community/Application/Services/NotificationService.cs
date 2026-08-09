using Microsoft.Extensions.Logging;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationBroadcaster _broadcaster;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            INotificationPreferenceRepository notificationPreferenceRepository,
            IUnitOfWork unitOfWork,
            INotificationBroadcaster broadcaster,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _notificationPreferenceRepository = notificationPreferenceRepository;
            _unitOfWork = unitOfWork;
            _broadcaster = broadcaster;
            _logger = logger;
        }

        public async Task<long> CreateAsync(NotificationCreateRequest request)
        {
            var entity = request.ToEntity();
            await _notificationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            if (await IsInAppEnabledAsync(entity.EmployeeId, entity.Category))
            {
                await BroadcastNotificationSafeAsync(entity.ToResponse());
                await BroadcastUnreadCountSafeAsync(entity.EmployeeId);
            }

            return entity.NotificationId;
        }

        public async Task MarkAsReadAsync(long notificationId)
        {
            var entity = await _notificationRepository.GetByIdAsync(notificationId)
                ?? throw new NotFoundException("Notification", notificationId);

            entity.IsRead = true;
            entity.ReadAt = DateTime.UtcNow;
            _notificationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            await BroadcastUnreadCountSafeAsync(entity.EmployeeId);
        }

        public async Task<int> GetUnreadCountAsync(long employeeId)
        {
            return await _notificationRepository.GetUnreadCountAsync(employeeId);
        }

        public async Task<int> MarkAllAsReadAsync(long employeeId)
        {
            var updatedCount = await _notificationRepository.MarkAllAsReadAsync(employeeId);
            await _unitOfWork.SaveChangesAsync();

            await BroadcastUnreadCountSafeAsync(employeeId);

            return updatedCount;
        }

        private async Task<bool> IsInAppEnabledAsync(long employeeId, string? category)
        {
            if (string.IsNullOrEmpty(category))
                return true;

            var preference = await _notificationPreferenceRepository.GetAsync(employeeId, category);

            // Absence of a preference row means the employee has never customized this
            // category, so it defaults to enabled.
            return preference?.InAppEnabled ?? true;
        }

        private async Task BroadcastNotificationSafeAsync(NotificationResponse notification)
        {
            try
            {
                await _broadcaster.BroadcastAsync(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast notification {NotificationId} to employee {EmployeeId}",
                    notification.NotificationId, notification.EmployeeId);
            }
        }

        private async Task BroadcastUnreadCountSafeAsync(long employeeId)
        {
            try
            {
                var unreadCount = await _notificationRepository.GetUnreadCountAsync(employeeId);
                await _broadcaster.BroadcastUnreadCountAsync(employeeId, unreadCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast unread count to employee {EmployeeId}", employeeId);
            }
        }

        public async Task DeleteAsync(long notificationId)
        {
            var entity = await _notificationRepository.GetByIdAsync(notificationId)
                ?? throw new NotFoundException("Notification", notificationId);

            _notificationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<NotificationResponse> GetByIdAsync(long notificationId)
        {
            var entity = await _notificationRepository.GetByIdAsync(notificationId)
                ?? throw new NotFoundException("Notification", notificationId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<NotificationResponse>> GetPaginatedAsync(long employeeId, NotificationFilterRequest request)
        {
            var pagedResult = await _notificationRepository.GetPaginatedByEmployeeAsync(employeeId, request);

            return new PagedResult<NotificationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
