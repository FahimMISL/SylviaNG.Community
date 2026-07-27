namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyAudienceResponse
    {
        public long AudienceId { get; set; }
        public long SurveyId { get; set; }
        public string AudienceType { get; set; } = string.Empty;
        public long? DepartmentId { get; set; }
        public long? BranchId { get; set; }
    }
}
