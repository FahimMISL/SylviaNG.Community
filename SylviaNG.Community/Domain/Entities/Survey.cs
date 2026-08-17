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

    /// <summary>
    /// When set, this survey has no locally-built SurveyQuestion rows - instead it links out to
    /// an external survey (e.g. a Google Form). "Taking" it means opening this URL and then
    /// self-reporting completion via the existing SubmitResponse endpoint with an empty answer
    /// set, so participation still shows up in SurveyResultsResponse without any new tracking
    /// mechanism.
    /// </summary>
    public string? ExternalUrl { get; set; }
}
