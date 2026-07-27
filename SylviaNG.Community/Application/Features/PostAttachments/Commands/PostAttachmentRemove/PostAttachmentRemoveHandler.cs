using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostAttachments.Commands.PostAttachmentRemove
{
    public class PostAttachmentRemoveHandler : IRequestHandler<PostAttachmentRemoveCommand>
    {
        private readonly IPostAttachmentService _postAttachmentService;

        public PostAttachmentRemoveHandler(IPostAttachmentService postAttachmentService)
        {
            _postAttachmentService = postAttachmentService;
        }

        public async Task Handle(PostAttachmentRemoveCommand command, CancellationToken cancellationToken)
        {
            await _postAttachmentService.RemoveAsync(command.PostId, command.AttachmentId);
        }
    }
}
