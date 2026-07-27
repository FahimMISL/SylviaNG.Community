using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ElectionVoteRepository : Repository<ElectionVote>, IElectionVoteRepository
    {
        public ElectionVoteRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<bool> HasVotedAsync(long electionId, long voterId)
        {
            return await _dbSet.AnyAsync(v => v.ElectionId == electionId && v.VoterId == voterId);
        }

        public async Task<PagedResult<ElectionVote>> GetPaginatedAsync(long electionId, PagedRequest request)
        {
            var query = _dbSet.Where(v => v.ElectionId == electionId);

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
