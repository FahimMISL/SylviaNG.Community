using MediatR;
using SylviaNG.Community.Application.Features.PostComments.Models;

namespace SylviaNG.Community.Application.Features.PostComments.Queries.PostCommentGetAll
{
    public class PostCommentGetAllQuery : IRequest<List<PostCommentResponse>>
    {
        public long PostId { get; set; }

        public PostCommentGetAllQuery(long postId)
        {
            PostId = postId;
        }
    }
}
