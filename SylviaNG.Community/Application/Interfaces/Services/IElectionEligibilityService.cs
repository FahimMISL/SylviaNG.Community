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
    }
}
