using MediatR;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationMarkRead
{
    public class NotificationMarkReadCommand : IRequest
    {
        public long NotificationId { get; set; }

        public NotificationMarkReadCommand(long notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
