using MediatR;
using SylviaNG.Community.Application.Features.PostComments.Models;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentAdd
{
    public class PostCommentAddCommand : IRequest<long>
    {
        public long PostId { get; set; }
        public PostCommentAddRequest Request { get; set; }

        public PostCommentAddCommand(long postId, PostCommentAddRequest request)
        {
            PostId = postId;
            Request = request;
        }
    }
}
