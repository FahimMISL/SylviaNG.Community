using MediatR;
using SylviaNG.Community.Application.Features.PostComments.Models;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentUpdate
{
    public class PostCommentUpdateCommand : IRequest
    {
        public long PostId { get; set; }
        public long CommentId { get; set; }
        public PostCommentUpdateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostCommentUpdateCommand(long postId, long commentId, PostCommentUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            CommentId = commentId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
