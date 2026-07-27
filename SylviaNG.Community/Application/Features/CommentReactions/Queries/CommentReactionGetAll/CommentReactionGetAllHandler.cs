using MediatR;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.CommentReactions.Queries.CommentReactionGetAll
{
    public class CommentReactionGetAllHandler : IRequestHandler<CommentReactionGetAllQuery, List<CommentReactionResponse>>
    {
        private readonly ICommentReactionService _commentReactionService;

        public CommentReactionGetAllHandler(ICommentReactionService commentReactionService)
        {
            _commentReactionService = commentReactionService;
        }

        public async Task<List<CommentReactionResponse>> Handle(CommentReactionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _commentReactionService.GetByCommentIdAsync(query.CommentId);
        }
    }
}
