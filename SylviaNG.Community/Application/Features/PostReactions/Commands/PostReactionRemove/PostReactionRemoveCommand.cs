using MediatR;

namespace SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionRemove
{
    public class PostReactionRemoveCommand : IRequest
    {
        public long PostId { get; set; }
        public long EmployeeId { get; set; }

        public PostReactionRemoveCommand(long postId, long employeeId)
        {
            PostId = postId;
            EmployeeId = employeeId;
        }
    }
}
