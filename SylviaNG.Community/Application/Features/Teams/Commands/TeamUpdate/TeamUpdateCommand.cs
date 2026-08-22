using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamUpdate
{
    public class TeamUpdateCommand : IRequest
    {
        public long TeamId { get; set; }
        public TeamUpdateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamUpdateCommand(long teamId, TeamUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            TeamId = teamId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
