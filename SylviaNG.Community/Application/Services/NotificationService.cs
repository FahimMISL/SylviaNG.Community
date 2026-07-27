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
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(NotificationCreateRequest request)
        {
            var entity = request.ToEntity();
            await _notificationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

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

        public async Task<PagedResult<NotificationResponse>> GetPaginatedAsync(long employeeId, PagedRequest request)
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
