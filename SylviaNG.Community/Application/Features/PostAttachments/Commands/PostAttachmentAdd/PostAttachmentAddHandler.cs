using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostAttachments.Commands.PostAttachmentAdd
{
    public class PostAttachmentAddHandler : IRequestHandler<PostAttachmentAddCommand, long>
    {
        private readonly IPostAttachmentService _postAttachmentService;

        public PostAttachmentAddHandler(IPostAttachmentService postAttachmentService)
        {
            _postAttachmentService = postAttachmentService;
        }

        public async Task<long> Handle(PostAttachmentAddCommand command, CancellationToken cancellationToken)
        {
            return await _postAttachmentService.AddAsync(command.PostId, command.Request);
        }
    }
}
