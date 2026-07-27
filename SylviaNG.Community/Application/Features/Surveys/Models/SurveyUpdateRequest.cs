namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? SurveyType { get; set; }
        public bool? IsAnonymous { get; set; }
        public bool? IsMandatory { get; set; }
    }
}
