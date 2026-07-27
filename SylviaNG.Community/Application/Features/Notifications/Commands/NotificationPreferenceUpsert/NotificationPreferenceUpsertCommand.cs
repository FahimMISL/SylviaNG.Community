using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationPreferenceUpsert
{
    public class NotificationPreferenceUpsertCommand : IRequest<long>
    {
        public NotificationPreferenceUpsertRequest Request { get; set; }

        public NotificationPreferenceUpsertCommand(NotificationPreferenceUpsertRequest request)
        {
            Request = request;
        }
    }
}
