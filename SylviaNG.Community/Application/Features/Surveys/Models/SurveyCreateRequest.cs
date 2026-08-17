namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SurveyType { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public bool IsMandatory { get; set; }
        public string? ExternalUrl { get; set; }
    }
}
