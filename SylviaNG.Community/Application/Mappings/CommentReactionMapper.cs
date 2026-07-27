using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class CommentReactionMapper
    {
        public static CommentReaction ToEntity(this CommentReactionAddRequest request, long commentId)
        {
            return new CommentReaction
            {
                CommentId = commentId,
                EmployeeId = request.EmployeeId,
                ReactionType = request.ReactionType
            };
        }

        public static CommentReactionResponse ToResponse(this CommentReaction entity)
        {
            return new CommentReactionResponse
            {
                ReactionId = entity.ReactionId,
                CommentId = entity.CommentId,
                EmployeeId = entity.EmployeeId,
                ReactionType = entity.ReactionType
            };
        }
    }
}
