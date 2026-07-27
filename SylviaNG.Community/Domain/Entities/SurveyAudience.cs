using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A targeting rule for a Survey. DepartmentId/BranchId are raw IDs with NO foreign-key
/// constraint or navigation property - Department/Branch are master data resolved live via
/// gRPC to the Core microservice, not local tables (same pattern as Employee.DepartmentId).
/// </summary>
public class SurveyAudience : Audit
{
    public long AudienceId { get; set; }
    public long SurveyId { get; set; }
    public string AudienceType { get; set; } = string.Empty;
    public long? DepartmentId { get; set; }
    public long? BranchId { get; set; }
}
