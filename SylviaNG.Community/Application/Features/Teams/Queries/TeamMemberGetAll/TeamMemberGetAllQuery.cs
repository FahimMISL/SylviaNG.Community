using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamMemberGetAll
{
    public class TeamMemberGetAllQuery : IRequest<List<TeamMemberResponse>>
    {
        public long TeamId { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamMemberGetAllQuery(long teamId, long? callerEmployeeId, bool isHrOrAdmin)
        {
            TeamId = teamId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
