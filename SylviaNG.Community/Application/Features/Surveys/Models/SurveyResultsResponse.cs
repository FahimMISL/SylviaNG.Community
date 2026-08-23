namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Aggregated results for a survey. ParticipationRate is only populated when the survey's
    /// audience is "EntireCompany" - Department/Branch-scoped surveys have no eligible-headcount
    /// source available yet, so it is left null (frontend should render "N/A").
    /// </summary>
    public class SurveyResultsResponse
    {
        public long SurveyId { get; set; }
        public int TotalResponses { get; set; }
        public decimal? ParticipationRate { get; set; }
        public List<SurveyQuestionResultResponse> Questions { get; set; } = new();
    }
}
