using Microsoft.AspNetCore.SignalR;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Maps a hub connection to the "employee_id" JWT claim so
    /// IHubContext&lt;NotificationHub, INotificationClient&gt;.Clients.User(employeeId) can target
    /// a specific employee's connection(s). Mirrors CurrentUserService's EmployeeIdClaimType.
    /// </summary>
    public class NotificationUserIdProvider : IUserIdProvider
    {
        private const string EmployeeIdClaimType = "employee_id";

        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(EmployeeIdClaimType)?.Value;
        }
    }
}
