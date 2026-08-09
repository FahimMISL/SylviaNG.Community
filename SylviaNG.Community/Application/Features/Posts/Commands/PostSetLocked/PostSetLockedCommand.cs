using MediatR;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostSetLocked
{
    public class PostSetLockedCommand : IRequest
    {
        public long PostId { get; set; }
        public bool IsLocked { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostSetLockedCommand(long postId, bool isLocked, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            IsLocked = isLocked;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
