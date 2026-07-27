using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetById
{
    public class NotificationGetByIdQuery : IRequest<NotificationResponse>
    {
        public long NotificationId { get; set; }

        public NotificationGetByIdQuery(long notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
