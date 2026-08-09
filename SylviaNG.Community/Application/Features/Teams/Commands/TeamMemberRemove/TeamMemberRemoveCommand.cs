using MediatR;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberRemove
{
    public class TeamMemberRemoveCommand : IRequest
    {
        public long TeamId { get; set; }
        public long EmployeeId { get; set; }

        public TeamMemberRemoveCommand(long teamId, long employeeId)
        {
            TeamId = teamId;
            EmployeeId = employeeId;
        }
    }
}
