using MediatR;
using SylviaNG.Community.Application.Features.PostReactions.Models;

namespace SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionAdd
{
    public class PostReactionAddCommand : IRequest<PostReactionResponse?>
    {
        public long PostId { get; set; }
        public PostReactionAddRequest Request { get; set; }

        public PostReactionAddCommand(long postId, PostReactionAddRequest request)
        {
            PostId = postId;
            Request = request;
        }
    }
}
