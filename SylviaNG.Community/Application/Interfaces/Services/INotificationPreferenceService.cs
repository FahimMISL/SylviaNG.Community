using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface INotificationPreferenceService
    {
        Task<long> UpsertAsync(NotificationPreferenceUpsertRequest request);
        Task<List<NotificationPreferenceResponse>> GetByEmployeeAsync(long employeeId);
    }
}
