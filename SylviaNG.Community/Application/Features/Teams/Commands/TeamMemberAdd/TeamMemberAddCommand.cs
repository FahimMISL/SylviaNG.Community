using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberAdd
{
    public class TeamMemberAddCommand : IRequest<long>
    {
        public long TeamId { get; set; }
        public TeamMemberAddRequest Request { get; set; }

        public TeamMemberAddCommand(long teamId, TeamMemberAddRequest request)
        {
            TeamId = teamId;
            Request = request;
        }
    }
}
