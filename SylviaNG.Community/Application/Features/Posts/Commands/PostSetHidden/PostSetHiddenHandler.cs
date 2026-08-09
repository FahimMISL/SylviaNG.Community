using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostSetHidden
{
    public class PostSetHiddenHandler : IRequestHandler<PostSetHiddenCommand>
    {
        private readonly IPostService _postService;

        public PostSetHiddenHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task Handle(PostSetHiddenCommand command, CancellationToken cancellationToken)
        {
            await _postService.SetHiddenAsync(command.PostId, command.IsHidden, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
