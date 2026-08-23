using MediatR;
using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Roles.Queries.RoleGetAllPaged
{
    public class RoleGetAllPagedQuery : IRequest<PagedResult<RoleResponse>>
    {
        public PagedRequest Request { get; set; }

        public RoleGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
