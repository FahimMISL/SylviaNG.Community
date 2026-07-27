using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// Who created the election is tracked via the inherited Audit.CreatedBy (long? -> Employee) -
/// no separate field needed.
///
/// ElectionType, CandidateType, AudienceScope and Status are kept as plain strings (not enums),
/// matching this codebase's convention for ERD-typed string fields.
///
/// Status convention (documented here since it is not an enum): expected values are
/// "Draft" (not yet open), "Open"/"Active" (voting allowed - see ElectionService.CastVoteAsync,
/// which accepts either string) and "Closed" (voting has ended). Callers should treat these
/// values case-sensitively unless a future migration normalizes them.
/// </summary>
public class Election : Audit
{
    public long ElectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ElectionType { get; set; } = string.Empty;
    public string CandidateType { get; set; } = string.Empty;
    public string AudienceScope { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public bool AllowMultipleChoice { get; set; }
    public int MinSelection { get; set; } = 1;
    public int MaxSelection { get; set; } = 1;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public new string Status { get; set; } = "Draft";
}
