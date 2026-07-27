namespace SylviaNG.Community.Application.Features.Recognitions.Models
{
    public class RecognitionCreateRequest
    {
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public string RecognitionType { get; set; } = string.Empty;
        public string? CoreValue { get; set; }
        public string? AwardTitle { get; set; }
        public string? Message { get; set; }
        public bool IsPublic { get; set; } = true;
    }
}
