using MediatR;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostDelete
{
    public class PostDeleteCommand : IRequest
    {
        public long PostId { get; set; }

        public PostDeleteCommand(long postId)
        {
            PostId = postId;
        }
    }
}
