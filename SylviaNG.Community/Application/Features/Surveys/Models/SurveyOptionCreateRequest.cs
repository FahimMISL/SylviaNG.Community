namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyOptionCreateRequest
    {
        public string OptionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
