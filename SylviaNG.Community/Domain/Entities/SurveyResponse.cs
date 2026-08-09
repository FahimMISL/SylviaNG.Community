using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A single employee's submitted response to a Survey (header row). The individual
/// answers are stored in SurveyAnswer - one row per question answered in this response.
/// </summary>
public class SurveyResponse : Audit
{
    public long ResponseId { get; set; }
    public long SurveyId { get; set; }
    public long EmployeeId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string CompletionStatus { get; set; } = string.Empty;
}
