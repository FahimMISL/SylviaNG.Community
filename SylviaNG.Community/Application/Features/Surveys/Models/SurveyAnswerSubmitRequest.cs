namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyAnswerSubmitRequest
    {
        public long QuestionId { get; set; }
        public long? OptionId { get; set; }
        public string? AnswerText { get; set; }
    }
}
