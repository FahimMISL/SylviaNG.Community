using MediatR;
using SylviaNG.Community.Application.Features.CommentReactions.Models;

namespace SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionAdd
{
    public class CommentReactionAddCommand : IRequest<CommentReactionResponse?>
    {
        public long CommentId { get; set; }
        public CommentReactionAddRequest Request { get; set; }

        public CommentReactionAddCommand(long commentId, CommentReactionAddRequest request)
        {
            CommentId = commentId;
            Request = request;
        }
    }
}
