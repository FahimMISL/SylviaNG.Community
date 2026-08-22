using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate
{
    public class TeamCreateHandler : IRequestHandler<TeamCreateCommand, long>
    {
        private readonly ITeamService _teamService;

        public TeamCreateHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<long> Handle(TeamCreateCommand command, CancellationToken cancellationToken)
        {
            return await _teamService.CreateAsync(command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
