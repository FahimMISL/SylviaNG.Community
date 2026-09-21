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

        /// <summary>
        /// Resolves the SignalR user identifier from the "employee_id" JWT claim only. Admin/system
        /// accounts authenticate without an employee_id claim, so this intentionally returns null for
        /// them - they are excluded from any Clients.User(...)-targeted push. This is by design, not a
        /// gap: admin/system accounts have no personal notifications or conversations to receive, so
        /// there is nothing for them to be targeted with. Do not "fix" this by falling back to another
        /// claim without first confirming admin/system accounts are meant to receive user-targeted pushes.
        /// </summary>
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(EmployeeIdClaimType)?.Value;
        }
    }
}
