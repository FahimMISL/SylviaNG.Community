using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Posts.Queries.PostGetById
{
    public class PostGetByIdHandler : IRequestHandler<PostGetByIdQuery, PostResponse>
    {
        private readonly IPostService _postService;

        public PostGetByIdHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<PostResponse> Handle(PostGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _postService.GetByIdAsync(query.PostId);
        }
    }
}
