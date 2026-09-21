using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IElectionService
    {
        Task<long> CreateAsync(ElectionCreateRequest request, long? createdBy);
        Task UpdateAsync(long electionId, ElectionUpdateRequest request);
        Task<ElectionResponse> GetByIdAsync(long electionId);
        Task<PagedResult<ElectionResponse>> GetPaginatedAsync(PagedRequest request);
        Task DeleteAsync(long electionId);

        Task PublishAsync(long electionId);
        Task CloseAsync(long electionId);

        /// <summary>Open elections the given employee is currently eligible to vote in (US-9.8).</summary>
        Task<List<ElectionEligibleResponse>> GetEligibleAsync(long employeeId);

        Task<long> AddAudienceTargetAsync(long electionId, ElectionAudienceTargetAddRequest request);
        Task<List<ElectionAudienceTargetResponse>> GetAudienceTargetsAsync(long electionId);

        Task<long> NominateAsync(long electionId, ElectionCandidateNominateRequest request);

        /// <summary>Nominates every active employee matching a scope (Organization/Branch/Department/Team) at once; returns the count newly nominated (already-nominated employees are skipped).</summary>
        Task<int> NominateBulkAsync(long electionId, ElectionCandidateNominateBulkRequest request);
        Task<List<ElectionCandidateResponse>> GetCandidatesAsync(long electionId);

        /// <summary>Removes a nomination. Blocked once the election has received any votes (a voted-for candidate cannot be deleted - see ElectionVoteConfiguration's Restrict delete behavior).</summary>
        Task RemoveCandidateAsync(long electionId, long candidateId);

        /// <summary>Adds or edits a candidate's manifesto after nomination.</summary>
        Task UpdateManifestoAsync(long electionId, long candidateId, ElectionCandidateUpdateManifestoRequest request);

        /// <summary>Casts one ballot (possibly selecting several candidates); returns the created vote row ids.</summary>
        Task<List<long>> CastVoteAsync(long electionId, ElectionVoteCastRequest request, long voterId);
        Task<PagedResult<ElectionVoteResponse>> GetVotesPaginatedAsync(long electionId, PagedRequest request);

        /// <summary>US-9.12: aggregated per-candidate totals, plus per-voter detail when the election isn't anonymous.</summary>
        Task<ElectionResultsResponse> GetResultsAsync(long electionId);
    }
}
