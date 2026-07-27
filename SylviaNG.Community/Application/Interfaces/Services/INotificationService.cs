using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<long> CreateAsync(NotificationCreateRequest request);
        Task MarkAsReadAsync(long notificationId);
        Task DeleteAsync(long notificationId);
        Task<NotificationResponse> GetByIdAsync(long notificationId);
        Task<PagedResult<NotificationResponse>> GetPaginatedAsync(long employeeId, PagedRequest request);
    }
}
