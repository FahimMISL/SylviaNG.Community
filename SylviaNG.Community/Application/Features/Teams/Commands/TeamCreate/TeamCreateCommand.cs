using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate
{
    public class TeamCreateCommand : IRequest<long>
    {
        public TeamCreateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamCreateCommand(TeamCreateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
