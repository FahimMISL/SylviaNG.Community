using MediatR;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionAdd
{
    public class PostReactionAddHandler : IRequestHandler<PostReactionAddCommand, PostReactionResponse?>
    {
        private readonly IPostReactionService _postReactionService;

        public PostReactionAddHandler(IPostReactionService postReactionService)
        {
            _postReactionService = postReactionService;
        }

        public async Task<PostReactionResponse?> Handle(PostReactionAddCommand command, CancellationToken cancellationToken)
        {
            return await _postReactionService.AddOrToggleAsync(command.PostId, command.Request);
        }
    }
}
