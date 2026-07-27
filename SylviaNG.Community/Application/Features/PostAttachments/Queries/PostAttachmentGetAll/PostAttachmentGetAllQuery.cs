using MediatR;
using SylviaNG.Community.Application.Features.PostAttachments.Models;

namespace SylviaNG.Community.Application.Features.PostAttachments.Queries.PostAttachmentGetAll
{
    public class PostAttachmentGetAllQuery : IRequest<List<PostAttachmentResponse>>
    {
        public long PostId { get; set; }

        public PostAttachmentGetAllQuery(long postId)
        {
            PostId = postId;
        }
    }
}
