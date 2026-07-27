using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate
{
    public class TeamCreateCommand : IRequest<long>
    {
        public TeamCreateRequest Request { get; set; }

        public TeamCreateCommand(TeamCreateRequest request)
        {
            Request = request;
        }
    }
}
