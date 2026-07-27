using MediatR;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationDelete
{
    public class NotificationDeleteCommand : IRequest
    {
        public long NotificationId { get; set; }

        public NotificationDeleteCommand(long notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
