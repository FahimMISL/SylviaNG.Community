using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Strongly-typed client contract for <see cref="NotificationHub"/>.
    /// Defines the methods the server can invoke on connected clients.
    /// </summary>
    public interface INotificationClient
    {
        // The trailing CancellationToken is SignalR's typed-client idiom: it is used only to
        // cancel the local send operation and is stripped before the call is sent over the wire.
        Task ReceiveNotification(NotificationResponse notification, CancellationToken cancellationToken = default);
        Task ReceiveUnreadCount(int unreadCount, CancellationToken cancellationToken = default);
    }
}
