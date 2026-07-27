using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class PostReactionMapper
    {
        public static PostReaction ToEntity(this PostReactionAddRequest request, long postId)
        {
            return new PostReaction
            {
                PostId = postId,
                EmployeeId = request.EmployeeId,
                ReactionType = request.ReactionType
            };
        }

        public static PostReactionResponse ToResponse(this PostReaction entity)
        {
            return new PostReactionResponse
            {
                ReactionId = entity.ReactionId,
                PostId = entity.PostId,
                EmployeeId = entity.EmployeeId,
                ReactionType = entity.ReactionType
            };
        }
    }
}
