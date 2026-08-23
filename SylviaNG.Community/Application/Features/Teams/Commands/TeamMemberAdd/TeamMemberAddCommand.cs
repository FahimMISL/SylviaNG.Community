using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberAdd
{
    public class TeamMemberAddCommand : IRequest<long>
    {
        public long TeamId { get; set; }
        public TeamMemberAddRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamMemberAddCommand(long teamId, TeamMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            TeamId = teamId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
