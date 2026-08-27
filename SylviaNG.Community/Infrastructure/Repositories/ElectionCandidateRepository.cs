using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ElectionCandidateRepository : Repository<ElectionCandidate>, IElectionCandidateRepository
    {
        public ElectionCandidateRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ElectionCandidate>> GetByElectionIdAsync(long electionId)
        {
            return await _dbSet.Where(c => c.ElectionId == electionId).ToListAsync();
        }

        public async Task<ElectionCandidate?> GetByIdForElectionAsync(long electionId, long candidateId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ElectionCandidateId == candidateId && c.ElectionId == electionId);
        }

        public async Task<List<ElectionCandidate>> GetByIdsForElectionAsync(long electionId, IEnumerable<long> candidateIds)
        {
            var idSet = candidateIds.ToList();
            return await _dbSet
                .Where(c => c.ElectionId == electionId && idSet.Contains(c.ElectionCandidateId))
                .ToListAsync();
        }

        public async Task<int> CountApprovedAsync(long electionId)
        {
            return await _dbSet.CountAsync(c => c.ElectionId == electionId && c.IsApproved);
        }
    }
}
