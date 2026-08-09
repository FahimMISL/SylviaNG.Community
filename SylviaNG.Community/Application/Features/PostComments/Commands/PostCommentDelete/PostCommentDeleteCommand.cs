using MediatR;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentDelete
{
    public class PostCommentDeleteCommand : IRequest
    {
        public long PostId { get; set; }
        public long CommentId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostCommentDeleteCommand(long postId, long commentId, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            CommentId = commentId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
