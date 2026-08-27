using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IElectionVoteRepository : IRepository<ElectionVote>
    {
        Task<bool> HasVotedAsync(long electionId, long voterId);
        Task<PagedResult<ElectionVote>> GetPaginatedAsync(long electionId, PagedRequest request);

        /// <summary>Whether the election has received any votes at all - drives the edit-lock rule once voting has started.</summary>
        Task<bool> HasAnyVotesAsync(long electionId);

        /// <summary>All votes for the election, unpaged - used to compute per-candidate result totals.</summary>
        Task<List<ElectionVote>> GetAllForElectionAsync(long electionId);
    }
}
