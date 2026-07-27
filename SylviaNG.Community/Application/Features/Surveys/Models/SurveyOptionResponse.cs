namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyOptionResponse
    {
        public long OptionId { get; set; }
        public long QuestionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
