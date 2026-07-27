using SylviaNG.Community.Application.Features.Elections.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class ElectionMapper
    {
        public static Election ToEntity(this ElectionCreateRequest request, long? createdBy)
        {
            return new Election
            {
                Title = request.Title,
                Description = request.Description,
                ElectionType = request.ElectionType,
                CandidateType = request.CandidateType,
                AudienceScope = request.AudienceScope,
                IsAnonymous = request.IsAnonymous,
                AllowMultipleChoice = request.AllowMultipleChoice,
                MinSelection = request.MinSelection,
                MaxSelection = request.MaxSelection,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = "Draft",
                CreatedBy = createdBy
            };
        }

        public static void ApplyUpdate(this Election entity, ElectionUpdateRequest request)
        {
            if (request.Title != null) entity.Title = request.Title;
            if (request.Description != null) entity.Description = request.Description;
            if (request.ElectionType != null) entity.ElectionType = request.ElectionType;
            if (request.CandidateType != null) entity.CandidateType = request.CandidateType;
            if (request.AudienceScope != null) entity.AudienceScope = request.AudienceScope;
            if (request.IsAnonymous.HasValue) entity.IsAnonymous = request.IsAnonymous.Value;
            if (request.AllowMultipleChoice.HasValue) entity.AllowMultipleChoice = request.AllowMultipleChoice.Value;
            if (request.MinSelection.HasValue) entity.MinSelection = request.MinSelection.Value;
            if (request.MaxSelection.HasValue) entity.MaxSelection = request.MaxSelection.Value;
            if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
            if (request.Status != null) entity.Status = request.Status;
        }

        public static ElectionResponse ToResponse(this Election entity)
        {
            return new ElectionResponse
            {
                ElectionId = entity.ElectionId,
                Title = entity.Title,
                Description = entity.Description,
                ElectionType = entity.ElectionType,
                CandidateType = entity.CandidateType,
                AudienceScope = entity.AudienceScope,
                IsAnonymous = entity.IsAnonymous,
                AllowMultipleChoice = entity.AllowMultipleChoice,
                MinSelection = entity.MinSelection,
                MaxSelection = entity.MaxSelection,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy
            };
        }

        public static ElectionAudienceTarget ToEntity(this ElectionAudienceTargetAddRequest request, long electionId)
        {
            return new ElectionAudienceTarget
            {
                ElectionId = electionId,
                TargetId = request.TargetId
            };
        }

        public static ElectionAudienceTargetResponse ToResponse(this ElectionAudienceTarget entity)
        {
            return new ElectionAudienceTargetResponse
            {
                ElectionAudienceTargetId = entity.ElectionAudienceTargetId,
                ElectionId = entity.ElectionId,
                TargetId = entity.TargetId
            };
        }

        public static ElectionCandidate ToEntity(this ElectionCandidateNominateRequest request, long electionId)
        {
            return new ElectionCandidate
            {
                ElectionId = electionId,
                EmployeeId = request.EmployeeId,
                TeamId = request.TeamId,
                CandidateType = request.CandidateType,
                Manifesto = request.Manifesto,
                IsApproved = false,
                NominatedAt = DateTime.UtcNow
            };
        }

        public static ElectionCandidateResponse ToResponse(this ElectionCandidate entity)
        {
            return new ElectionCandidateResponse
            {
                ElectionCandidateId = entity.ElectionCandidateId,
                ElectionId = entity.ElectionId,
                EmployeeId = entity.EmployeeId,
                TeamId = entity.TeamId,
                CandidateType = entity.CandidateType,
                Manifesto = entity.Manifesto,
                IsApproved = entity.IsApproved,
                NominatedAt = entity.NominatedAt
            };
        }

        public static ElectionVote ToEntity(this ElectionVoteCastRequest request, long electionId, long voterId)
        {
            return new ElectionVote
            {
                ElectionId = electionId,
                CandidateId = request.CandidateId,
                VoterId = voterId,
                VotedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Hides VoterId when the owning Election is anonymous - see the tradeoff comment on
        /// ElectionVoteResponse.VoterId.
        /// </summary>
        public static ElectionVoteResponse ToResponse(this ElectionVote entity, bool isAnonymous)
        {
            return new ElectionVoteResponse
            {
                ElectionVoteId = entity.ElectionVoteId,
                ElectionId = entity.ElectionId,
                CandidateId = entity.CandidateId,
                VoterId = isAnonymous ? null : entity.VoterId,
                VotedAt = entity.VotedAt
            };
        }
    }
}
