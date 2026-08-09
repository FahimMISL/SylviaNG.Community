using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamDelete
{
    public class TeamDeleteHandler : IRequestHandler<TeamDeleteCommand>
    {
        private readonly ITeamService _teamService;

        public TeamDeleteHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task Handle(TeamDeleteCommand command, CancellationToken cancellationToken)
        {
            await _teamService.DeleteAsync(command.TeamId);
        }
    }
}
