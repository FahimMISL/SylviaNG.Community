using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A nominated candidate in an Election - either an individual Employee or a Team,
/// depending on the owning Election's CandidateType. Exactly one of EmployeeId/TeamId
/// is expected to be set at a time (enforced in ElectionCandidateNominateValidator).
/// IsApproved defaults to false on nomination and is only flipped via the HRAdminOnly
/// approve endpoint (see ElectionController).
/// </summary>
public class ElectionCandidate : Audit
{
    public long ElectionCandidateId { get; set; }
    public long ElectionId { get; set; }
    public long? EmployeeId { get; set; }
    public long? TeamId { get; set; }
    public string CandidateType { get; set; } = string.Empty;
    public string? Manifesto { get; set; }
    public bool IsApproved { get; set; }
    public DateTime NominatedAt { get; set; }
}
