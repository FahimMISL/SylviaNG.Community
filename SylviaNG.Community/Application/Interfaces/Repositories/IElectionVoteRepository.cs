using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionVoteRepository : IRepository<ElectionVote>
    {
        Task<bool> HasVotedAsync(long electionId, long voterId);
        Task<PagedResult<ElectionVote>> GetPaginatedAsync(long electionId, PagedRequest request);
    }
}
