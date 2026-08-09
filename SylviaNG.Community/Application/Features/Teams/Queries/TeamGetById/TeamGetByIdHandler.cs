using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetById
{
    public class TeamGetByIdHandler : IRequestHandler<TeamGetByIdQuery, TeamResponse>
    {
        private readonly ITeamService _teamService;

        public TeamGetByIdHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<TeamResponse> Handle(TeamGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _teamService.GetByIdAsync(query.TeamId);
        }
    }
}
