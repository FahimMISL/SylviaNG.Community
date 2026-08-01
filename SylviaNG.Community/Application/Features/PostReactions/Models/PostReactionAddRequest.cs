using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.PostReactions.Models
{
    public class PostReactionAddRequest
    {
        public long EmployeeId { get; set; }
        public ReactionTypeEnum ReactionType { get; set; }
    }
}
