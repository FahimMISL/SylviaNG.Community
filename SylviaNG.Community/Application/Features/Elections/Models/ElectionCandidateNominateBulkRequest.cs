namespace SylviaNG.Community.Application.Features.Elections.Models
{
    /// <summary>
    /// Bulk-nominates every active employee matching a scope (ElectionConstants.ElectionAudienceScope:
    /// Organization/Branch/Department/Team) as a candidate, instead of nominating one employee at a
    /// time. TargetIds is ignored for Organization scope.
    /// </summary>
    public class ElectionCandidateNominateBulkRequest
    {
        public string Scope { get; set; } = string.Empty;
        public List<long> TargetIds { get; set; } = new();
    }
}
