using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamMemberGetAll
{
    public class TeamMemberGetAllQuery : IRequest<List<TeamMemberResponse>>
    {
        public long TeamId { get; set; }

        public TeamMemberGetAllQuery(long teamId)
        {
            TeamId = teamId;
        }
    }
}
