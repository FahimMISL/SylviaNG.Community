using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetById
{
    public class TeamGetByIdQuery : IRequest<TeamResponse>
    {
        public long TeamId { get; set; }

        public TeamGetByIdQuery(long teamId)
        {
            TeamId = teamId;
        }
    }
}
