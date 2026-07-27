using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;

namespace SylviaNG.Community.Application.Features.Posts.Queries.PostGetById
{
    public class PostGetByIdQuery : IRequest<PostResponse>
    {
        public long PostId { get; set; }

        public PostGetByIdQuery(long postId)
        {
            PostId = postId;
        }
    }
}
