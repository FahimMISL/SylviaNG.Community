using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationDelete
{
    public class NotificationDeleteHandler : IRequestHandler<NotificationDeleteCommand>
    {
        private readonly INotificationService _notificationService;

        public NotificationDeleteHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(NotificationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _notificationService.DeleteAsync(command.NotificationId);
        }
    }
}
