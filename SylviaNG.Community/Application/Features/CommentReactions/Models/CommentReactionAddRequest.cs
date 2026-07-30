using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.CommentReactions.Models
{
    public class CommentReactionAddRequest
    {
        public long EmployeeId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
    }
}
