namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyQuestionResponse
    {
        public long QuestionId { get; set; }
        public long SurveyId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
        public List<SurveyOptionResponse> Options { get; set; } = new();
    }
}
