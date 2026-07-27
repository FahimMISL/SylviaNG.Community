namespace SylviaNG.Community.Application.Features.CommentReactions.Models
{
    public class CommentReactionResponse
    {
        public long ReactionId { get; set; }
        public long CommentId { get; set; }
        public long EmployeeId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
    }
}
