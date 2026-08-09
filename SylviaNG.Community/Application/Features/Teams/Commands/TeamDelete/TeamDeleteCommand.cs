using MediatR;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamDelete
{
    public class TeamDeleteCommand : IRequest
    {
        public long TeamId { get; set; }

        public TeamDeleteCommand(long teamId)
        {
            TeamId = teamId;
        }
    }
}
