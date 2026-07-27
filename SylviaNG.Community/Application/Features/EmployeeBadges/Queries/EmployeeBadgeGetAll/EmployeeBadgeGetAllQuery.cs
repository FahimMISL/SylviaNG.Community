using MediatR;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;

namespace SylviaNG.Community.Application.Features.EmployeeBadges.Queries.EmployeeBadgeGetAll
{
    public class EmployeeBadgeGetAllQuery : IRequest<List<EmployeeBadgeResponse>>
    {
        public long EmployeeId { get; set; }

        public EmployeeBadgeGetAllQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
