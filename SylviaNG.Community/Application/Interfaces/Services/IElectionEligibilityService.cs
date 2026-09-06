using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IElectionEligibilityService
    {
        /// <summary>
        /// Resolves the set of active employee ids eligible to vote in the given election,
        /// based on its AudienceScope and the ElectionAudienceTarget rows configuring it.
        /// </summary>
        Task<HashSet<long>> GetEligibleEmployeeIdsAsync(Election election, List<ElectionAudienceTarget> targets);

        /// <summary>
        /// Resolves active employee ids for a scope (ElectionConstants.ElectionAudienceScope) and
        /// target ids directly, independent of any particular Election - used by voter-eligibility
        /// resolution above.
        /// </summary>
        Task<HashSet<long>> ResolveEmployeeIdsAsync(string scope, List<long> targetIds);

        /// <summary>
        /// Same resolution as ResolveEmployeeIdsAsync, but for Team scope also folds in each
        /// team's supervisor (Team.SupervisorId) alongside its regular TeamMember rows - used by
        /// bulk candidate nomination (ElectionService.NominateBulkAsync), where "everyone in this
        /// team" is expected to include its lead, not just rank-and-file members. Voter
        /// eligibility deliberately keeps the narrower ResolveEmployeeIdsAsync behavior instead.
        /// </summary>
        Task<HashSet<long>> ResolveCandidateEmployeeIdsAsync(string scope, List<long> targetIds);
    }
}
