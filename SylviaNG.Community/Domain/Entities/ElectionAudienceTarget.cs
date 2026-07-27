using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

/// <summary>
/// A single targeting rule for an Election's audience. Kept generic per the source ERD:
/// TargetId is a free-text identifier (e.g. could hold a department/branch/site id as a
/// string) rather than a typed FK, since the meaning of the id depends on the owning
/// Election's AudienceScope (e.g. "Department", "Site", "All").
/// </summary>
public class ElectionAudienceTarget : Audit
{
    public long ElectionAudienceTargetId { get; set; }
    public long ElectionId { get; set; }
    public string TargetId { get; set; } = string.Empty;
}
