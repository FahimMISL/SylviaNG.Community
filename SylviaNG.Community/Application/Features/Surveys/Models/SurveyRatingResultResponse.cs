namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Aggregate for a Rating-type question: the mean of all submitted RatingValue answers, plus
    /// a count per distinct value (e.g. { 1: 0, 2: 1, 3: 4, 4: 6, 5: 2 }) so the frontend can render
    /// a distribution instead of an unlabeled list of raw digits.
    /// </summary>
    public class SurveyRatingResultResponse
    {
        public decimal AverageValue { get; set; }
        public Dictionary<int, int> Distribution { get; set; } = new();
    }
}
