using MediatR;
using SylviaNG.Community.Application.Features.Notifications.Models;

namespace SylviaNG.Community.Application.Features.Notifications.Queries.NotificationPreferenceGetAllByEmployee
{
    public class NotificationPreferenceGetAllByEmployeeQuery : IRequest<List<NotificationPreferenceResponse>>
    {
        public long EmployeeId { get; set; }

        public NotificationPreferenceGetAllByEmployeeQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
