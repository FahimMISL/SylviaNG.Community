using MediatR;
using SylviaNG.Community.Application.Features.PostAttachments.Models;

namespace SylviaNG.Community.Application.Features.PostAttachments.Commands.PostAttachmentAdd
{
    public class PostAttachmentAddCommand : IRequest<long>
    {
        public long PostId { get; set; }
        public PostAttachmentAddRequest Request { get; set; }

        public PostAttachmentAddCommand(long postId, PostAttachmentAddRequest request)
        {
            PostId = postId;
            Request = request;
        }
    }
}
