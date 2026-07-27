namespace SylviaNG.Community.Application.Features.Elections.Models
{
    public class ElectionResponse
    {
        public long ElectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ElectionType { get; set; } = string.Empty;
        public string CandidateType { get; set; } = string.Empty;
        public string AudienceScope { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public bool AllowMultipleChoice { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? CreatedBy { get; set; }
    }
}
