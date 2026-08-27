using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionCandidateRepository : IRepository<ElectionCandidate>
    {
        Task<List<ElectionCandidate>> GetByElectionIdAsync(long electionId);
        Task<ElectionCandidate?> GetByIdForElectionAsync(long electionId, long candidateId);

        /// <summary>Fetches multiple candidates belonging to the election in one round trip - used to validate a multi-select ballot atomically.</summary>
        Task<List<ElectionCandidate>> GetByIdsForElectionAsync(long electionId, IEnumerable<long> candidateIds);

        /// <summary>Approved-candidate count, used to gate publishing (must be &gt;= Election.MinSelection).</summary>
        Task<int> CountApprovedAsync(long electionId);
    }
}
