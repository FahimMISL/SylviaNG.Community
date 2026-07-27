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

        Task<long> AddAudienceTargetAsync(long electionId, ElectionAudienceTargetAddRequest request);
        Task<List<ElectionAudienceTargetResponse>> GetAudienceTargetsAsync(long electionId);

        Task<long> NominateAsync(long electionId, ElectionCandidateNominateRequest request);
        Task<List<ElectionCandidateResponse>> GetCandidatesAsync(long electionId);
        Task ApproveCandidateAsync(long electionId, long candidateId);

        Task<long> CastVoteAsync(long electionId, ElectionVoteCastRequest request, long voterId);
        Task<PagedResult<ElectionVoteResponse>> GetVotesPaginatedAsync(long electionId, PagedRequest request);
    }
}
