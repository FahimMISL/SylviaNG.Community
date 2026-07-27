using MediatR;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostSetHidden
{
    public class PostSetHiddenCommand : IRequest
    {
        public long PostId { get; set; }
        public bool IsHidden { get; set; }

        public PostSetHiddenCommand(long postId, bool isHidden)
        {
            PostId = postId;
            IsHidden = isHidden;
        }
    }
}
