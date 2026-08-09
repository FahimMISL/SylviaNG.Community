using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationPreferenceGetAllByEmployee
{
    public class NotificationPreferenceGetAllByEmployeeHandler : IRequestHandler<NotificationPreferenceGetAllByEmployeeQuery, List<NotificationPreferenceResponse>>
    {
        private readonly INotificationPreferenceService _notificationPreferenceService;

        public NotificationPreferenceGetAllByEmployeeHandler(INotificationPreferenceService notificationPreferenceService)
        {
            _notificationPreferenceService = notificationPreferenceService;
        }

        public async Task<List<NotificationPreferenceResponse>> Handle(NotificationPreferenceGetAllByEmployeeQuery query, CancellationToken cancellationToken)
        {
            return await _notificationPreferenceService.GetByEmployeeAsync(query.EmployeeId);
        }
    }
}
