using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationMarkRead
{
    public class NotificationMarkReadHandler : IRequestHandler<NotificationMarkReadCommand>
    {
        private readonly INotificationService _notificationService;

        public NotificationMarkReadHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(NotificationMarkReadCommand command, CancellationToken cancellationToken)
        {
            await _notificationService.MarkAsReadAsync(command.NotificationId);
        }
    }
}
