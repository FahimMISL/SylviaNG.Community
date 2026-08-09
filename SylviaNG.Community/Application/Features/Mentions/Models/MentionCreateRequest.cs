namespace SylviaNG.Community.Application.Features.Mentions.Models
{
    public class MentionCreateRequest
    {
        public long MentionedEmployeeId { get; set; }
        public long MentionedBy { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public long EntityId { get; set; }
    }
}
