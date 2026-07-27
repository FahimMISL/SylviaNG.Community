using MediatR;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostSetLocked
{
    public class PostSetLockedCommand : IRequest
    {
        public long PostId { get; set; }
        public bool IsLocked { get; set; }

        public PostSetLockedCommand(long postId, bool isLocked)
        {
            PostId = postId;
            IsLocked = isLocked;
        }
    }
}
