using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetByEmployeeId
{
    public class TeamGetByEmployeeIdQuery : IRequest<List<TeamResponse>>
    {
        public long EmployeeId { get; set; }

        public TeamGetByEmployeeIdQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
