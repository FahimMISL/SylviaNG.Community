using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamMemberGetAll
{
    public class TeamMemberGetAllHandler : IRequestHandler<TeamMemberGetAllQuery, List<TeamMemberResponse>>
    {
        private readonly ITeamService _teamService;

        public TeamMemberGetAllHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<List<TeamMemberResponse>> Handle(TeamMemberGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _teamService.GetMembersAsync(query.TeamId);
        }
    }
}
