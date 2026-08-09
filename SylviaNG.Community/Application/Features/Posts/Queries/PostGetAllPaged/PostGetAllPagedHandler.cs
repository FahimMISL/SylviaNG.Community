using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Posts.Queries.PostGetAllPaged
{
    public class PostGetAllPagedHandler : IRequestHandler<PostGetAllPagedQuery, PagedResult<PostResponse>>
    {
        private readonly IPostService _postService;

        public PostGetAllPagedHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<PagedResult<PostResponse>> Handle(PostGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _postService.GetFeedPaginatedAsync(query.Request, query.CallerEmployeeId);
        }
    }
}
