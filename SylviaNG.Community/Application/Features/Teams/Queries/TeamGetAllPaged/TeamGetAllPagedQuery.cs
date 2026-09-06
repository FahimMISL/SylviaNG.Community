using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetAllPaged
{
    public class TeamGetAllPagedQuery : IRequest<PagedResult<TeamResponse>>
    {
        public PagedRequest Request { get; set; }
        public long? CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public TeamGetAllPagedQuery(PagedRequest request, long? callerEmployeeId, bool isHrOrAdmin)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
