using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetAllPaged
{
    public class NotificationGetAllPagedQuery : IRequest<PagedResult<NotificationResponse>>
    {
        public long EmployeeId { get; set; }
        public NotificationFilterRequest Request { get; set; }

        public NotificationGetAllPagedQuery(long employeeId, NotificationFilterRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
