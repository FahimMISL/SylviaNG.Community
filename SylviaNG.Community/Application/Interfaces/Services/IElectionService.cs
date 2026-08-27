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
        Task<List<ElectionCandidateResponse>> GetCandidatesAsync(long electionId);
        Task ApproveCandidateAsync(long electionId, long candidateId);

        /// <summary>Casts one ballot (possibly selecting several candidates); returns the created vote row ids.</summary>
        Task<List<long>> CastVoteAsync(long electionId, ElectionVoteCastRequest request, long voterId);
        Task<PagedResult<ElectionVoteResponse>> GetVotesPaginatedAsync(long electionId, PagedRequest request);

        /// <summary>US-9.12: aggregated per-candidate totals, plus per-voter detail when the election isn't anonymous.</summary>
        Task<ElectionResultsResponse> GetResultsAsync(long electionId);
    }
}
