using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate
{
    public class PostUpdateCommand : IRequest
    {
        public long PostId { get; set; }
        public PostUpdateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public PostUpdateCommand(long postId, PostUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            PostId = postId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
