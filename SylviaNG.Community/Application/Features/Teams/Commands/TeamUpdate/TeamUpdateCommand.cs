using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamUpdate
{
    public class TeamUpdateCommand : IRequest
    {
        public long TeamId { get; set; }
        public TeamUpdateRequest Request { get; set; }

        public TeamUpdateCommand(long teamId, TeamUpdateRequest request)
        {
            TeamId = teamId;
            Request = request;
        }
    }
}
