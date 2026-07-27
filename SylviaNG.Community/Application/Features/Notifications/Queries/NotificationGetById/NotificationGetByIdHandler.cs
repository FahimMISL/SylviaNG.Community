using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetById
{
    public class NotificationGetByIdHandler : IRequestHandler<NotificationGetByIdQuery, NotificationResponse>
    {
        private readonly INotificationService _notificationService;

        public NotificationGetByIdHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<NotificationResponse> Handle(NotificationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _notificationService.GetByIdAsync(query.NotificationId);
        }
    }
}
