using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetByEmployeeId
{
    public class TeamGetByEmployeeIdHandler : IRequestHandler<TeamGetByEmployeeIdQuery, List<TeamResponse>>
    {
        private readonly ITeamService _teamService;

        public TeamGetByEmployeeIdHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<List<TeamResponse>> Handle(TeamGetByEmployeeIdQuery query, CancellationToken cancellationToken)
        {
            return await _teamService.GetTeamsByEmployeeIdAsync(query.EmployeeId);
        }
    }
}
