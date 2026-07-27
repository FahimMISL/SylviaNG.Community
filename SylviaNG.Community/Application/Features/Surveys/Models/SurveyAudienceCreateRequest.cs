namespace SylviaNG.Community.Application.Features.Surveys.Models
{
    public class SurveyAudienceCreateRequest
    {
        public string AudienceType { get; set; } = string.Empty;
        public long? DepartmentId { get; set; }
        public long? BranchId { get; set; }
    }
}
