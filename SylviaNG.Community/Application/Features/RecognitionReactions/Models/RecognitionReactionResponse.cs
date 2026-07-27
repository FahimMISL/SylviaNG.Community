namespace SylviaNG.Community.Application.Features.RecognitionReactions.Models
{
    public class RecognitionReactionResponse
    {
        public long ReactionId { get; set; }
        public long RecognitionId { get; set; }
        public long EmployeeId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
    }
}
