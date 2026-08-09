using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetUnreadCount
{
    public class NotificationGetUnreadCountHandler : IRequestHandler<NotificationGetUnreadCountQuery, int>
    {
        private readonly INotificationService _notificationService;

        public NotificationGetUnreadCountHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<int> Handle(NotificationGetUnreadCountQuery query, CancellationToken cancellationToken)
        {
            return await _notificationService.GetUnreadCountAsync(query.EmployeeId);
        }
    }
}
