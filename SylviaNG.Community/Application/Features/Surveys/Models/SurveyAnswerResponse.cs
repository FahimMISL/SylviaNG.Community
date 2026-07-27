namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyAnswerResponse
    {
        public long AnswerId { get; set; }
        public long ResponseId { get; set; }
        public long QuestionId { get; set; }
        public long? OptionId { get; set; }
        public string? AnswerText { get; set; }
    }
}
