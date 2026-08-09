using MediatR;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Groups.Queries.GroupGetAllPaged
{
    public class GroupGetAllPagedQuery : IRequest<PagedResult<GroupResponse>>
    {
        public PagedRequest Request { get; set; }

        public GroupGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
