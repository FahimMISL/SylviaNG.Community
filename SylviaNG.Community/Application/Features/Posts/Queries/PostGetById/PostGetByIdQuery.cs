using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;

namespace SylviaNG.Community.Application.Features.Posts.Queries.PostGetById
{
    public class PostGetByIdQuery : IRequest<PostResponse>
    {
        public long PostId { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostGetByIdQuery(long postId, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
