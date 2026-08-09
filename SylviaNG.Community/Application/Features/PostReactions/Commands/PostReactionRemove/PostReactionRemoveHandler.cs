using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionRemove
{
    public class PostReactionRemoveHandler : IRequestHandler<PostReactionRemoveCommand>
    {
        private readonly IPostReactionService _postReactionService;

        public PostReactionRemoveHandler(IPostReactionService postReactionService)
        {
            _postReactionService = postReactionService;
        }

        public async Task Handle(PostReactionRemoveCommand command, CancellationToken cancellationToken)
        {
            await _postReactionService.RemoveAsync(command.PostId, command.EmployeeId);
        }
    }
}
