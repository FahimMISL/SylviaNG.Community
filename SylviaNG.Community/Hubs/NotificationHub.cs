using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Receive/broadcast-only hub for v1 - notifications are pushed to clients via
    /// <see cref="SylviaNG.Community.Application.Interfaces.Services.INotificationBroadcaster"/>.
    /// No client-invoked mutation methods here; mark-read/mark-all-read still go through
    /// the REST endpoints in NotificationController so validation stays in one place.
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
    }
}
