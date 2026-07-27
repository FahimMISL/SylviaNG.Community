using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate
{
    public class PostUpdateCommand : IRequest
    {
        public long PostId { get; set; }
        public PostUpdateRequest Request { get; set; }

        public PostUpdateCommand(long postId, PostUpdateRequest request)
        {
            PostId = postId;
            Request = request;
        }
    }
}
