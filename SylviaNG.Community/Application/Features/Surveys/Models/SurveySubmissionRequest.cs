namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    /// <summary>
    /// Request body for POST community/survey/{surveyId}/responses - submits a full survey
    /// response (the SurveyResponse header plus every SurveyAnswer row) in a single call.
    /// </summary>
    public class SurveySubmissionRequest
    {
        public long EmployeeId { get; set; }
        public List<SurveyAnswerSubmitRequest> Answers { get; set; } = new();
    }
}
