using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberRemove
{
    public class TeamMemberRemoveHandler : IRequestHandler<TeamMemberRemoveCommand>
    {
        private readonly ITeamService _teamService;

        public TeamMemberRemoveHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task Handle(TeamMemberRemoveCommand command, CancellationToken cancellationToken)
        {
            await _teamService.RemoveMemberAsync(command.TeamId, command.EmployeeId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
