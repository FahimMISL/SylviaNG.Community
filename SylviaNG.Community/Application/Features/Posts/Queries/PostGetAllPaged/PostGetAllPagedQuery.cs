using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Posts.Queries.PostGetAllPaged
{
    public class PostGetAllPagedQuery : IRequest<PagedResult<PostResponse>>
    {
        public PagedRequest Request { get; set; }

        public PostGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
