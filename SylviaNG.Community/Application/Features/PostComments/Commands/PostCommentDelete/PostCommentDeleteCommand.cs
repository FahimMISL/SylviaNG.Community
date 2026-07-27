using MediatR;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentDelete
{
    public class PostCommentDeleteCommand : IRequest
    {
        public long PostId { get; set; }
        public long CommentId { get; set; }

        public PostCommentDeleteCommand(long postId, long commentId)
        {
            PostId = postId;
            CommentId = commentId;
        }
    }
}
