namespace SylviaNG.Community.Application.Features.PostReactions.Models
{
    public class PostReactionResponse
    {
        public long ReactionId { get; set; }
        public long PostId { get; set; }
        public long EmployeeId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
    }
}
