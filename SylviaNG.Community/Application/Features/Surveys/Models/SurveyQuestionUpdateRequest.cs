namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyQuestionUpdateRequest
    {
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsRequired { get; set; }
    }
}
