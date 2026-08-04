namespace SylviaNG.Community.Application.Features.Recognitions.Models
{
    public class RecognitionResponse
    {
        public long RecognitionId { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public long? BadgeId { get; set; }
        public string? BadgeName { get; set; }
        public string? BadgeIcon { get; set; }
        public string? BadgeColor { get; set; }
        public string RecognitionType { get; set; } = string.Empty;
        public string? CoreValue { get; set; }
        public string? AwardTitle { get; set; }
        public string? Message { get; set; }
        public bool IsPublic { get; set; }
        public bool IsHrIssued { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
