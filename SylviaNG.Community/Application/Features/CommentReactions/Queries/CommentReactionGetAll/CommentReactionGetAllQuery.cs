using MediatR;
using SylviaNG.Community.Application.Features.CommentReactions.Models;

namespace SylviaNG.Community.Application.Features.CommentReactions.Queries.CommentReactionGetAll
{
    public class CommentReactionGetAllQuery : IRequest<List<CommentReactionResponse>>
    {
        public long CommentId { get; set; }

        public CommentReactionGetAllQuery(long commentId)
        {
            CommentId = commentId;
        }
    }
}
