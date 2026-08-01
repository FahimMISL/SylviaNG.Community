using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.CommentReactions.Models
{
    public class CommentReactionResponse
    {
        public long ReactionId { get; set; }
        public long CommentId { get; set; }
        public long EmployeeId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
    }
}
