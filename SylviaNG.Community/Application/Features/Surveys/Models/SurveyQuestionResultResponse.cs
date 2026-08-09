namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Per-question breakdown: Options is populated for choice-type questions (with counts and
    /// percentages of TotalResponses), TextAnswers is populated for free-text questions.
    /// </summary>
    public class SurveyQuestionResultResponse
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public List<SurveyOptionResultResponse> Options { get; set; } = new();
        public List<string> TextAnswers { get; set; } = new();
    }
}
