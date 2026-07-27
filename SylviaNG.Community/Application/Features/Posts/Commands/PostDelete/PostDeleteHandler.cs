using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostDelete
{
    public class PostDeleteHandler : IRequestHandler<PostDeleteCommand>
    {
        private readonly IPostService _postService;

        public PostDeleteHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task Handle(PostDeleteCommand command, CancellationToken cancellationToken)
        {
            await _postService.DeleteAsync(command.PostId);
        }
    }
}
