using MediatR;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;

namespace SylviaNG.Community.Application.Features.EmployeeBadges.Commands.EmployeeBadgeAward
{
    public class EmployeeBadgeAwardCommand : IRequest<long>
    {
        public long EmployeeId { get; set; }
        public EmployeeBadgeAwardRequest Request { get; set; }

        public EmployeeBadgeAwardCommand(long employeeId, EmployeeBadgeAwardRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
