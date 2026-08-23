using MediatR;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberRemove
{
    public class TeamMemberRemoveCommand : IRequest
    {
        public long TeamId { get; set; }
        public long EmployeeId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamMemberRemoveCommand(long teamId, long employeeId, long callerEmployeeId, bool isHrOrAdmin)
        {
            TeamId = teamId;
            EmployeeId = employeeId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
