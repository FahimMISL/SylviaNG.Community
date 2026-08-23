namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Per-question breakdown: Options is populated for choice-type questions (with counts and
    /// percentages of the number of respondents who answered THIS question, not the survey's
    /// total response count), TextAnswers is populated for free-text questions, and Rating is
    /// populated for Rating-type questions (average + per-value distribution).
    /// </summary>
    public class SurveyQuestionResultResponse
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public List<SurveyOptionResultResponse> Options { get; set; } = new();
        public List<string> TextAnswers { get; set; } = new();
        public SurveyRatingResultResponse? Rating { get; set; }
    }
}
