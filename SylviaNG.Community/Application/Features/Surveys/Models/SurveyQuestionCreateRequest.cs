namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyQuestionCreateRequest
    {
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
        public List<SurveyOptionCreateRequest> Options { get; set; } = new();
    }
}
