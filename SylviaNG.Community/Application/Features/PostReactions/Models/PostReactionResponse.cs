using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.PostReactions.Models
{
    public class PostReactionResponse
    {
        public long ReactionId { get; set; }
        public long PostId { get; set; }
        public long EmployeeId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
    }
}
