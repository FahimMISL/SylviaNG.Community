namespace SylviaNG.Community.Application.Features.PostReactions.Models
{
    public class PostReactionAddRequest
    {
        public long EmployeeId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
    }
}
