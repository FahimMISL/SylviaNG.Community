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
        public long EmployeeId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string CompletionStatus { get; set; } = string.Empty;
        public List<SurveyAnswerResponse> Answers { get; set; } = new();
    }
}
