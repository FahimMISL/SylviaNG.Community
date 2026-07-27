using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetAllPaged
{
    public class NotificationGetAllPagedHandler : IRequestHandler<NotificationGetAllPagedQuery, PagedResult<NotificationResponse>>
    {
        private readonly INotificationService _notificationService;

        public NotificationGetAllPagedHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<PagedResult<NotificationResponse>> Handle(NotificationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _notificationService.GetPaginatedAsync(query.EmployeeId, query.Request);
        }
    }
}
