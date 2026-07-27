namespace SylviaNG.Community.Application.Features.CommentReactions.Models
{
    public class CommentReactionAddRequest
    {
        public long EmployeeId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
    }
}
