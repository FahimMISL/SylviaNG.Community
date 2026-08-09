using Microsoft.AspNetCore.SignalR;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Hubs;

namespace SylviaNG.Community.Infrastructure.Services
{
    /// <summary>
    /// Lives in Infrastructure because it depends on Microsoft.AspNetCore.SignalR
    /// (same placement logic as JwtTokenGenerator).
    /// </summary>
    public class SignalRNotificationBroadcaster : INotificationBroadcaster
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public SignalRNotificationBroadcaster(IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastAsync(NotificationResponse notification, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(notification.EmployeeId.ToString()).ReceiveNotification(notification, cancellationToken);
        }

        public async Task BroadcastUnreadCountAsync(long employeeId, int unreadCount, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(employeeId.ToString()).ReceiveUnreadCount(unreadCount, cancellationToken);
        }
    }
}
