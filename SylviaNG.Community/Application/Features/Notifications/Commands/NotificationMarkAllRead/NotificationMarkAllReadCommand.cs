using MediatR;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationMarkAllRead
{
    public class NotificationMarkAllReadCommand : IRequest<int>
    {
        public long EmployeeId { get; set; }

        public NotificationMarkAllReadCommand(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
