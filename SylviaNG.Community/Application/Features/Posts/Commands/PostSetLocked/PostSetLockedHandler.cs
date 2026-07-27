using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostSetLocked
{
    public class PostSetLockedHandler : IRequestHandler<PostSetLockedCommand>
    {
        private readonly IPostService _postService;

        public PostSetLockedHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task Handle(PostSetLockedCommand command, CancellationToken cancellationToken)
        {
            await _postService.SetLockedAsync(command.PostId, command.IsLocked);
        }
    }
}
