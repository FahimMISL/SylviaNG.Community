using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationCreate
{
    public class NotificationCreateHandler : IRequestHandler<NotificationCreateCommand, long>
    {
        private readonly INotificationService _notificationService;

        public NotificationCreateHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<long> Handle(NotificationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _notificationService.CreateAsync(command.Request);
        }
    }
}
