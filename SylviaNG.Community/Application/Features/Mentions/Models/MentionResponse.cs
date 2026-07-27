namespace SylviaNG.Community.Application.Features.Mentions.Models
{
    public class MentionResponse
    {
        public long MentionId { get; set; }
        public long MentionedEmployeeId { get; set; }
        public long MentionedBy { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public long EntityId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
