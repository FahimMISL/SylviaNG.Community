using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Who created the survey is tracked via the inherited Audit.CreatedBy - no separate field needed.
/// Status hides the inherited Audit.Status (int) with a plain string workflow status
/// (e.g. "Draft", "Published", "Closed") per the ERD, mirroring Announcement.Status.
/// </summary>
public class Survey : Audit
{
    public long SurveyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SurveyType { get; set; } = string.Empty;
    public new string Status { get; set; } = "Draft";
    public bool IsAnonymous { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}
