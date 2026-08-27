using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Constants;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class ElectionService : IElectionService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IElectionAudienceTargetRepository _audienceTargetRepository;
        private readonly IElectionCandidateRepository _candidateRepository;
        private readonly IElectionVoteRepository _voteRepository;
        private readonly IElectionEligibilityService _eligibilityService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public ElectionService(
            IElectionRepository electionRepository,
            IElectionAudienceTargetRepository audienceTargetRepository,
            IElectionCandidateRepository candidateRepository,
            IElectionVoteRepository voteRepository,
            IElectionEligibilityService eligibilityService,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _electionRepository = electionRepository;
            _audienceTargetRepository = audienceTargetRepository;
            _candidateRepository = candidateRepository;
            _voteRepository = voteRepository;
            _eligibilityService = eligibilityService;
            _notificationService = notificationService;
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

            var hasVotes = await _voteRepository.HasAnyVotesAsync(electionId);
            if (hasVotes)
            {
                var lockedFields = request.GetLockedFieldsBeingChanged();
                if (lockedFields.Count > 0)
                    throw new ForbiddenException(
                        $"Election \"{entity.Title}\" has already received votes - {string.Join(", ", lockedFields)} can no longer be changed. Only Title, Description and EndDate remain editable.");

                entity.ApplyRestrictedUpdate(request);
            }
            else
            {
                entity.ApplyUpdate(request);
            }

            _electionRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long electionId)
        {
            var entity = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            if (entity.Status != ElectionStatus.Draft)
                throw new ForbiddenException($"Election \"{entity.Title}\" has already been published - only draft elections can be deleted.");

            await _audienceTargetRepository.DeleteWhereAsync(t => t.ElectionId == electionId);
            await _candidateRepository.DeleteWhereAsync(c => c.ElectionId == electionId);
            _electionRepository.Delete(entity);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PublishAsync(long electionId)
        {
            var entity = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            if (entity.Status != ElectionStatus.Draft)
                throw new ForbiddenException($"Election \"{entity.Title}\" has already been published.");

            var targets = await _audienceTargetRepository.GetByElectionIdAsync(electionId);
            if (entity.AudienceScope != ElectionAudienceScope.Organization && targets.Count == 0)
                throw new ForbiddenException("Publishing is blocked until a voting scope is configured.");

            var approvedCandidateCount = await _candidateRepository.CountApprovedAsync(electionId);
            if (approvedCandidateCount < entity.MinSelection)
                throw new ForbiddenException(
                    $"At least {entity.MinSelection} approved candidate(s) are required to publish - currently {approvedCandidateCount}.");

            entity.Status = ElectionStatus.Open;
            _electionRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var eligibleEmployeeIds = await _eligibilityService.GetEligibleEmployeeIdsAsync(entity, targets);
            foreach (var employeeId in eligibleEmployeeIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = employeeId,
                    Title = $"New election open for voting: {entity.Title}",
                    Category = "Election",
                    RelatedEntityType = "Election",
                    RelatedEntityId = entity.ElectionId
                });
            }
        }

        public async Task CloseAsync(long electionId)
        {
            var entity = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            if (!ElectionStatus.Votable.Contains(entity.Status, StringComparer.OrdinalIgnoreCase))
                throw new ForbiddenException($"Election \"{entity.Title}\" is not currently open (status: \"{entity.Status}\").");

            entity.Status = ElectionStatus.Closed;
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

        public async Task<List<ElectionEligibleResponse>> GetEligibleAsync(long employeeId)
        {
            var openElections = await _electionRepository.GetByStatusAsync(ElectionStatus.Open);
            var now = DateTime.UtcNow;
            var result = new List<ElectionEligibleResponse>();

            foreach (var election in openElections)
            {
                if (election.EndDate.HasValue && now > election.EndDate.Value)
                    continue;

                var targets = await _audienceTargetRepository.GetByElectionIdAsync(election.ElectionId);
                var eligibleEmployeeIds = await _eligibilityService.GetEligibleEmployeeIdsAsync(election, targets);
                if (!eligibleEmployeeIds.Contains(employeeId))
                    continue;

                var hasVoted = await _voteRepository.HasVotedAsync(election.ElectionId, employeeId);
                result.Add(election.ToEligibleResponse(hasVoted));
            }

            return result;
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

        public async Task<List<long>> CastVoteAsync(long electionId, ElectionVoteCastRequest request, long voterId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            if (!ElectionStatus.Votable.Contains(election.Status, StringComparer.OrdinalIgnoreCase))
                throw new ForbiddenException($"Election \"{election.Title}\" is not currently open for voting (status: \"{election.Status}\").");

            var now = DateTime.UtcNow;
            if (now < election.StartDate || (election.EndDate.HasValue && now > election.EndDate.Value))
                throw new ForbiddenException($"Election \"{election.Title}\" is not within its voting window.");

            var targets = await _audienceTargetRepository.GetByElectionIdAsync(electionId);
            var eligibleEmployeeIds = await _eligibilityService.GetEligibleEmployeeIdsAsync(election, targets);
            if (!eligibleEmployeeIds.Contains(voterId))
                throw new ForbiddenException("You are not eligible to vote in this election.");

            var hasVoted = await _voteRepository.HasVotedAsync(electionId, voterId);
            if (hasVoted)
                throw new DuplicateException("ElectionVote", "VoterId", voterId.ToString());

            var selectedCount = request.CandidateIds.Count;
            if (selectedCount < election.MinSelection || selectedCount > election.MaxSelection)
                throw new ForbiddenException(
                    $"Select between {election.MinSelection} and {election.MaxSelection} candidate(s) - {selectedCount} selected.");

            var distinctCandidateIds = request.CandidateIds.Distinct().ToList();
            var candidates = await _candidateRepository.GetByIdsForElectionAsync(electionId, distinctCandidateIds);
            if (candidates.Count != distinctCandidateIds.Count)
                throw new NotFoundException("ElectionCandidate", string.Join(",", distinctCandidateIds));

            if (candidates.Any(c => !c.IsApproved))
                throw new ForbiddenException("One or more selected candidates have not been approved and cannot receive votes.");

            var entities = request.ToEntities(electionId, voterId);
            await _voteRepository.AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync();

            return entities.Select(v => v.ElectionVoteId).ToList();
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

        public async Task<ElectionResultsResponse> GetResultsAsync(long electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId)
                ?? throw new NotFoundException("Election", electionId);

            var votes = await _voteRepository.GetAllForElectionAsync(electionId);
            var candidates = await _candidateRepository.GetByElectionIdAsync(electionId);

            var tallies = candidates.Select(c => new ElectionCandidateTally
            {
                ElectionCandidateId = c.ElectionCandidateId,
                EmployeeId = c.EmployeeId,
                TeamId = c.TeamId,
                VoteCount = votes.Count(v => v.CandidateId == c.ElectionCandidateId)
            }).ToList();

            var response = new ElectionResultsResponse
            {
                ElectionId = election.ElectionId,
                IsAnonymous = election.IsAnonymous,
                Status = election.Status,
                TotalVotes = votes.Count,
                CandidateTallies = tallies
            };

            // Never compute voter-level detail for anonymous elections, so there is nothing to leak.
            if (!election.IsAnonymous)
            {
                response.VoterDetails = votes
                    .GroupBy(v => v.VoterId)
                    .Select(g => new ElectionVoterDetail
                    {
                        VoterId = g.Key,
                        CandidateIds = g.Select(v => v.CandidateId).ToList(),
                        VotedAt = g.Min(v => v.VotedAt)
                    })
                    .ToList();
            }

            return response;
        }
    }
}
