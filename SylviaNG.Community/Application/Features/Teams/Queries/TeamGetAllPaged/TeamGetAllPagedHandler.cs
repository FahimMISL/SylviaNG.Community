using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetAllPaged
{
    public class TeamGetAllPagedHandler : IRequestHandler<TeamGetAllPagedQuery, PagedResult<TeamResponse>>
    {
        private readonly ITeamService _teamService;

        public TeamGetAllPagedHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<PagedResult<TeamResponse>> Handle(TeamGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _teamService.GetPaginatedAsync(query.Request);
        }
    }
}
