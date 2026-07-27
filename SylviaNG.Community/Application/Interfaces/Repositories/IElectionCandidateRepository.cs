using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionCandidateRepository : IRepository<ElectionCandidate>
    {
        Task<List<ElectionCandidate>> GetByElectionIdAsync(long electionId);
        Task<ElectionCandidate?> GetByIdForElectionAsync(long electionId, long candidateId);
    }
}
