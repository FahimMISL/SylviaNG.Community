using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionRemove
{
    public class CommentReactionRemoveHandler : IRequestHandler<CommentReactionRemoveCommand>
    {
        private readonly ICommentReactionService _commentReactionService;

        public CommentReactionRemoveHandler(ICommentReactionService commentReactionService)
        {
            _commentReactionService = commentReactionService;
        }

        public async Task Handle(CommentReactionRemoveCommand command, CancellationToken cancellationToken)
        {
            await _commentReactionService.RemoveAsync(command.CommentId, command.EmployeeId);
        }
    }
}
