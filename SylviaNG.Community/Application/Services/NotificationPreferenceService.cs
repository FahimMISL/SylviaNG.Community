using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly INotificationPreferenceRepository _notificationPreferenceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationPreferenceService(
            INotificationPreferenceRepository notificationPreferenceRepository,
            IUnitOfWork unitOfWork)
        {
            _notificationPreferenceRepository = notificationPreferenceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> UpsertAsync(NotificationPreferenceUpsertRequest request)
        {
            var existing = await _notificationPreferenceRepository.GetAsync(request.EmployeeId, request.Category);

            if (existing != null)
            {
                existing.ApplyUpdate(request);
                _notificationPreferenceRepository.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                return existing.PreferenceId;
            }

            var entity = request.ToEntity();
            await _notificationPreferenceRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.PreferenceId;
        }

        public async Task<List<NotificationPreferenceResponse>> GetByEmployeeAsync(long employeeId)
        {
            var preferences = await _notificationPreferenceRepository.GetByEmployeeAsync(employeeId);
            return preferences.Select(p => p.ToResponse()).ToList();
        }
    }
}
