using MediatR;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Teams.Queries.TeamGetAllPaged
{
    public class TeamGetAllPagedQuery : IRequest<PagedResult<TeamResponse>>
    {
        public PagedRequest Request { get; set; }

        public TeamGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
