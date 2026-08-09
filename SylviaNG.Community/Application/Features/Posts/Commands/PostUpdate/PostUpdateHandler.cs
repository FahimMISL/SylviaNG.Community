using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate
{
    public class PostUpdateHandler : IRequestHandler<PostUpdateCommand>
    {
        private readonly IPostService _postService;

        public PostUpdateHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task Handle(PostUpdateCommand command, CancellationToken cancellationToken)
        {
            await _postService.UpdateAsync(command.PostId, command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
