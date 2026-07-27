using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationPreferenceUpsert
{
    public class NotificationPreferenceUpsertHandler : IRequestHandler<NotificationPreferenceUpsertCommand, long>
    {
        private readonly INotificationPreferenceService _notificationPreferenceService;

        public NotificationPreferenceUpsertHandler(INotificationPreferenceService notificationPreferenceService)
        {
            _notificationPreferenceService = notificationPreferenceService;
        }

        public async Task<long> Handle(NotificationPreferenceUpsertCommand command, CancellationToken cancellationToken)
        {
            return await _notificationPreferenceService.UpsertAsync(command.Request);
        }
    }
}
