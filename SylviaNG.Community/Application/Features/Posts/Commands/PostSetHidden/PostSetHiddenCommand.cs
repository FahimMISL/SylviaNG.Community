using MediatR;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostSetHidden
{
    public class PostSetHiddenCommand : IRequest
    {
        public long PostId { get; set; }
        public bool IsHidden { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostSetHiddenCommand(long postId, bool isHidden, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            IsHidden = isHidden;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
