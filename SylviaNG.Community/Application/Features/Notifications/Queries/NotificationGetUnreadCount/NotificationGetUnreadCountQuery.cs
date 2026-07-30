using MediatR;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetUnreadCount
{
    public class NotificationGetUnreadCountQuery : IRequest<int>
    {
        public long EmployeeId { get; set; }

        public NotificationGetUnreadCountQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
