using MediatR;

namespace SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionRemove
{
    public class CommentReactionRemoveCommand : IRequest
    {
        public long CommentId { get; set; }
        public long EmployeeId { get; set; }

        public CommentReactionRemoveCommand(long commentId, long employeeId)
        {
            CommentId = commentId;
            EmployeeId = employeeId;
        }
    }
}
