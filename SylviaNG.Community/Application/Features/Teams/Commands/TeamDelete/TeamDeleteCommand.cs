using MediatR;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamDelete
{
    public class TeamDeleteCommand : IRequest
    {
        public long TeamId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamDeleteCommand(long teamId, long callerEmployeeId, bool isHrOrAdmin)
        {
            TeamId = teamId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
