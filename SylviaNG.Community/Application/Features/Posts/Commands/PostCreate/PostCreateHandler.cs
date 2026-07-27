using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostCreate
{
    public class PostCreateHandler : IRequestHandler<PostCreateCommand, long>
    {
        private readonly IPostService _postService;

        public PostCreateHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<long> Handle(PostCreateCommand command, CancellationToken cancellationToken)
        {
            return await _postService.CreateAsync(command.Request);
        }
    }
}
