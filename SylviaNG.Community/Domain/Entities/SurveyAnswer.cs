using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// OptionId is set only for choice-type questions; free-text questions leave it null
/// and populate AnswerText instead. RatingValue is set only for Rating-type questions -
/// kept as its own numeric column (rather than stored in AnswerText) so results aggregation
/// can compute an average/distribution instead of just listing raw digit strings.
/// </summary>
public class SurveyAnswer : Audit
{
    public long AnswerId { get; set; }
    public long ResponseId { get; set; }
    public long QuestionId { get; set; }
    public long? OptionId { get; set; }
    public string? AnswerText { get; set; }
    public int? RatingValue { get; set; }
}
