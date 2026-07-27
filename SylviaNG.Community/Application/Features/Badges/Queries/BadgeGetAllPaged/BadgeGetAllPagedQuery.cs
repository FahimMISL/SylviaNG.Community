using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetAllPaged
{
    public class BadgeGetAllPagedQuery : IRequest<PagedResult<BadgeResponse>>
    {
        public PagedRequest Request { get; set; }

        public BadgeGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
