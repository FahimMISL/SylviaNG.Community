using MediatR;
using SylviaNG.Community.Application.Features.PostAttachments.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostAttachments.Queries.PostAttachmentGetAll
{
    public class PostAttachmentGetAllHandler : IRequestHandler<PostAttachmentGetAllQuery, List<PostAttachmentResponse>>
    {
        private readonly IPostAttachmentService _postAttachmentService;

        public PostAttachmentGetAllHandler(IPostAttachmentService postAttachmentService)
        {
            _postAttachmentService = postAttachmentService;
        }

        public async Task<List<PostAttachmentResponse>> Handle(PostAttachmentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _postAttachmentService.GetByPostIdAsync(query.PostId);
        }
    }
}
