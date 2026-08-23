using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberAdd
{
    public class TeamMemberAddHandler : IRequestHandler<TeamMemberAddCommand, long>
    {
        private readonly ITeamService _teamService;

        public TeamMemberAddHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<long> Handle(TeamMemberAddCommand command, CancellationToken cancellationToken)
        {
            return await _teamService.AddMemberAsync(command.TeamId, command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
