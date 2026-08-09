namespace SylviaNG.Community.Application.Features.Recognitions.Models
{
    public class RecognitionCreateRequest
    {
        public long RecipientId { get; set; }
        public List<long> BadgeIds { get; set; } = new();
        public string RecognitionType { get; set; } = string.Empty;
        public string? CoreValue { get; set; }
        public string? AwardTitle { get; set; }
        public string? Message { get; set; }
        public bool IsPublic { get; set; } = true;

        /// <summary>Requests this recognition be marked a formal HR/Admin-issued award; only honored if the caller is HR/Admin.</summary>
        public bool IsHrIssued { get; set; } = false;
    }
}
