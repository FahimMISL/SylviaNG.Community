namespace SylviaNG.Community.Application.Features.RecognitionReactions.Models
{
    public class RecognitionReactionAddRequest
    {
        public long EmployeeId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
    }
}
