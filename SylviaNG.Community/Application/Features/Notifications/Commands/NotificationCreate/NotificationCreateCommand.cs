using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationCreate
{
    public class NotificationCreateCommand : IRequest<long>
    {
        public NotificationCreateRequest Request { get; set; }

        public NotificationCreateCommand(NotificationCreateRequest request)
        {
            Request = request;
        }
    }
}
