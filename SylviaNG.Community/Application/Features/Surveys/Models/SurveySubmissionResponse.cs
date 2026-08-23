namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Read model for the Domain.Entities.SurveyResponse entity (an employee's submitted
    /// response, including its answers). Named "SurveySubmissionResponse" rather than
    /// "SurveyResponseResponse" for readability.
    /// </summary>
    public class SurveySubmissionResponse
    {
        public long ResponseId { get; set; }
        public long SurveyId { get; set; }

        /// <summary>
        /// Null when the parent Survey.IsAnonymous is true - see SurveyMapper.ToResponse(SurveyResponse,...).
        /// The EmployeeId is still stored on SurveyResponse for anonymous surveys (needed for the
        /// one-response-per-employee uniqueness constraint) but is never exposed through this DTO.
        /// </summary>
        public long? EmployeeId { get; set; }

        public DateTime SubmittedAt { get; set; }
        public string CompletionStatus { get; set; } = string.Empty;
        public List<SurveyAnswerResponse> Answers { get; set; } = new();
    }
}
