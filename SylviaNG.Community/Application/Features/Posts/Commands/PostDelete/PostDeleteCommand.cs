using MediatR;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostDelete
{
    public class PostDeleteCommand : IRequest
    {
        public long PostId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostDeleteCommand(long postId, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
