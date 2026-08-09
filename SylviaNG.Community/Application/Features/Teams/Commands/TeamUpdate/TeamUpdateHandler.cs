using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamUpdate
{
    public class TeamUpdateHandler : IRequestHandler<TeamUpdateCommand>
    {
        private readonly ITeamService _teamService;

        public TeamUpdateHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task Handle(TeamUpdateCommand command, CancellationToken cancellationToken)
        {
            await _teamService.UpdateAsync(command.TeamId, command.Request);
        }
    }
}
