using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// OptionId is set only for choice-type questions; free-text questions leave it null
/// and populate AnswerText instead.
/// </summary>
public class SurveyAnswer : Audit
{
    public long AnswerId { get; set; }
    public long ResponseId { get; set; }
    public long QuestionId { get; set; }
    public long? OptionId { get; set; }
    public string? AnswerText { get; set; }
}
