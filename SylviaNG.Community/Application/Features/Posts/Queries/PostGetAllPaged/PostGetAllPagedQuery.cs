using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Posts.Queries.PostGetAllPaged
{
    public class PostGetAllPagedQuery : IRequest<PagedResult<PostResponse>>
    {
        public PostFilterRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public PostGetAllPagedQuery(PostFilterRequest request, long callerEmployeeId)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
