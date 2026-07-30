using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    /// <summary>
    /// Pushes real-time notification events to connected clients over SignalR.
    /// Implemented in Infrastructure (SignalRNotificationBroadcaster) since the concrete
    /// implementation depends on Microsoft.AspNetCore.SignalR.
    /// </summary>
    public interface INotificationBroadcaster
    {
        Task BroadcastAsync(NotificationResponse notification, CancellationToken cancellationToken = default);
        Task BroadcastUnreadCountAsync(long employeeId, int unreadCount, CancellationToken cancellationToken = default);
    }
}
