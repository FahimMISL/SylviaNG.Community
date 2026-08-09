using MediatR;
using SylviaNG.Community.Application.Features.PostReactions.Models;

namespace SylviaNG.Community.Application.Features.PostReactions.Queries.PostReactionGetAll
{
    public class PostReactionGetAllQuery : IRequest<List<PostReactionResponse>>
    {
        public long PostId { get; set; }

        public PostReactionGetAllQuery(long postId)
        {
            PostId = postId;
        }
    }
}
