namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Read model for the Survey entity itself. Named "SurveyDetailResponse" rather than the
    /// usual "{Entity}Response" convention (see TeamResponse/AnnouncementResponse) because
    /// "SurveyResponse" is already taken by the Domain.Entities.SurveyResponse entity, which
    /// represents an employee's submitted answers - a different concept entirely.
    /// </summary>
    public class SurveyDetailResponse
    {
        public long SurveyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SurveyType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public bool IsMandatory { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? ExternalUrl { get; set; }
    }
}
