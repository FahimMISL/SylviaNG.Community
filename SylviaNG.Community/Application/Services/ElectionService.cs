using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class ElectionService : IElectionService
    {
        /// <summary>
        /// Status values (plain string, see the convention documented on the Election entity)
        /// that permit a vote to be cast right now.
        /// </summary>
        private static readonly string[] VotableStatuses = { "Open", "Active" };

        private readonly IElectionRepository _electionRepository;
        private readonly IElectionAudienceTargetRepository _audienceTargetRepository;
        private readonly IElectionCandidateRepository _candidateRepository;
        private readonly IElectionVoteRepository _voteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ElectionService(
            IElectionRepository electionRepository,
            IElectionAudienceTargetRepository audienceTargetRepository,
            IElectionCandidateRepository candidateRepository,
            IElectionVoteRepository voteRepository,
            IUnitOfWork unitOfWork)
        {
            _electionRepository = electionRepository;
            _audienceTargetRepository = audienceTargetRepository;
            _candidateRepository = candidateRepository;
            _voteRepository = voteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ElectionCreateRequest request, long? createdBy)
        {
            var entity = request.ToEntity(createdBy);
            await _electionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ElectionId;
        }

        public async Task UpdateAsync(long electionId, ElectionUpdateRequest request)
        {
            var entity = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            entity.ApplyUpdate(request);
            _electionRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ElectionResponse> GetByIdAsync(long electionId)
        {
            var entity = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<ElectionResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _electionRepository.GetPaginatedAsync(request);

            return new PagedResult<ElectionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AddAudienceTargetAsync(long electionId, ElectionAudienceTargetAddRequest request)
        {
            _ = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            var entity = request.ToEntity(electionId);
            await _audienceTargetRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ElectionAudienceTargetId;
        }

        public async Task<List<ElectionAudienceTargetResponse>> GetAudienceTargetsAsync(long electionId)
        {
            var targets = await _audienceTargetRepository.GetByElectionIdAsync(electionId);
            return targets.Select(t => t.ToResponse()).ToList();
        }

        public async Task<long> NominateAsync(long electionId, ElectionCandidateNominateRequest request)
        {
            _ = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            var entity = request.ToEntity(electionId);
            await _candidateRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ElectionCandidateId;
        }

        public async Task<List<ElectionCandidateResponse>> GetCandidatesAsync(long electionId)
        {
            var candidates = await _candidateRepository.GetByElectionIdAsync(electionId);
            return candidates.Select(c => c.ToResponse()).ToList();
        }

        public async Task ApproveCandidateAsync(long electionId, long candidateId)
        {
            var candidate = await _candidateRepository.GetByIdForElectionAsync(electionId, candidateId)
                ?? throw new NotFoundException("ElectionCandidate", candidateId);

            candidate.IsApproved = true;
            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<long> CastVoteAsync(long electionId, ElectionVoteCastRequest request, long voterId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            if (!VotableStatuses.Contains(election.Status, StringComparer.OrdinalIgnoreCase))
                throw new ForbiddenException($"Election \"{election.Title}\" is not currently open for voting (status: \"{election.Status}\").");

            var now = DateTime.UtcNow;
            if (now < election.StartDate || (election.EndDate.HasValue && now > election.EndDate.Value))
                throw new ForbiddenException($"Election \"{election.Title}\" is not within its voting window.");

            var candidate = await _candidateRepository.GetByIdForElectionAsync(electionId, request.CandidateId)
                ?? throw new NotFoundException("ElectionCandidate", request.CandidateId);

            if (!candidate.IsApproved)
                throw new ForbiddenException("This candidate has not been approved and cannot receive votes.");

            if (!election.AllowMultipleChoice)
            {
                var hasVoted = await _voteRepository.HasVotedAsync(electionId, voterId);
                if (hasVoted)
                    throw new DuplicateException("ElectionVote", "VoterId", voterId.ToString());
            }

            var entity = request.ToEntity(electionId, voterId);
            await _voteRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ElectionVoteId;
        }

        public async Task<PagedResult<ElectionVoteResponse>> GetVotesPaginatedAsync(long electionId, PagedRequest request)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            var pagedResult = await _voteRepository.GetPaginatedAsync(electionId, request);

            return new PagedResult<ElectionVoteResponse>
            {
                Data = pagedResult.Data.Select(v => v.ToResponse(election.IsAnonymous)).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
