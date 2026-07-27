using MediatR;

namespace SylviaNG.Community.Application.Features.PostAttachments.Commands.PostAttachmentRemove
{
    public class PostAttachmentRemoveCommand : IRequest
    {
        public long PostId { get; set; }
        public long AttachmentId { get; set; }

        public PostAttachmentRemoveCommand(long postId, long attachmentId)
        {
            PostId = postId;
            AttachmentId = attachmentId;
        }
    }
}
