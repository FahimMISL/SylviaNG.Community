using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Commands.NotificationMarkAllRead
{
    public class NotificationMarkAllReadHandler : IRequestHandler<NotificationMarkAllReadCommand, int>
    {
        private readonly INotificationService _notificationService;

        public NotificationMarkAllReadHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<int> Handle(NotificationMarkAllReadCommand command, CancellationToken cancellationToken)
        {
            return await _notificationService.MarkAllAsReadAsync(command.EmployeeId);
        }
    }
}
