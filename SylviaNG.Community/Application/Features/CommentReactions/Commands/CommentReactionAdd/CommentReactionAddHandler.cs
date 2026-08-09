using MediatR;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionAdd
{
    public class CommentReactionAddHandler : IRequestHandler<CommentReactionAddCommand, CommentReactionResponse?>
    {
        private readonly ICommentReactionService _commentReactionService;

        public CommentReactionAddHandler(ICommentReactionService commentReactionService)
        {
            _commentReactionService = commentReactionService;
        }

        public async Task<CommentReactionResponse?> Handle(CommentReactionAddCommand command, CancellationToken cancellationToken)
        {
            return await _commentReactionService.AddOrToggleAsync(command.CommentId, command.Request);
        }
    }
}
