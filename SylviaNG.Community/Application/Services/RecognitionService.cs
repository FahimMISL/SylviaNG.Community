using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class RecognitionService : IRecognitionService
    {
        private readonly IRecognitionRepository _recognitionRepository;
        private readonly IRecognitionReactionRepository _recognitionReactionRepository;
        private readonly IRecognitionCommentRepository _recognitionCommentRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecognitionService(
            IRecognitionRepository recognitionRepository,
            IRecognitionReactionRepository recognitionReactionRepository,
            IRecognitionCommentRepository recognitionCommentRepository,
            IBadgeRepository badgeRepository,
            IUnitOfWork unitOfWork)
        {
            _recognitionRepository = recognitionRepository;
            _recognitionReactionRepository = recognitionReactionRepository;
            _recognitionCommentRepository = recognitionCommentRepository;
            _badgeRepository = badgeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RecognitionCreateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            if (callerEmployeeId <= 0)
                throw new ForbiddenException("A valid employee identity is required to send a recognition.");

            if (request.IsHrIssued && !isHrOrAdmin)
                throw new ForbiddenException("Only HR/Admin can issue a formal award.");

            if (request.BadgeId.HasValue)
            {
                _ = await _badgeRepository.GetByIdAsync(request.BadgeId.Value)
                    ?? throw new NotFoundException("Badge", request.BadgeId.Value);
            }

            var entity = request.ToEntity(callerEmployeeId);
            await _recognitionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.RecognitionId;
        }

        public async Task<RecognitionResponse> GetByIdAsync(long recognitionId)
        {
            var entity = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var badge = entity.BadgeId.HasValue
                ? await _badgeRepository.GetByIdAsync(entity.BadgeId.Value)
                : null;

            return entity.ToResponse(badge);
        }

        public async Task<PagedResult<RecognitionResponse>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null)
        {
            var pagedResult = await _recognitionRepository.GetPaginatedAsync(request, senderId, recipientId);

            var badgeIds = pagedResult.Data
                .Where(e => e.BadgeId.HasValue)
                .Select(e => e.BadgeId!.Value)
                .Distinct()
                .ToList();

            var badgesById = badgeIds.Count == 0
                ? new Dictionary<long, Badge>()
                : (await _badgeRepository.FindAsync(b => badgeIds.Contains(b.BadgeId)))
                    .ToDictionary(b => b.BadgeId);

            return new PagedResult<RecognitionResponse>
            {
                Data = pagedResult.Data
                    .Select(e => e.ToResponse(e.BadgeId.HasValue && badgesById.TryGetValue(e.BadgeId.Value, out var badge) ? badge : null))
                    .ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AddReactionAsync(long recognitionId, RecognitionReactionAddRequest request, long callerEmployeeId)
        {
            if (callerEmployeeId <= 0)
                throw new ForbiddenException("A valid employee identity is required to react.");

            _ = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var existing = await _recognitionReactionRepository.GetAsync(recognitionId, callerEmployeeId);
            if (existing != null)
            {
                existing.ReactionType = request.ReactionType;
                _recognitionReactionRepository.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                return existing.ReactionId;
            }

            var entity = request.ToEntity(recognitionId, callerEmployeeId);
            await _recognitionReactionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReactionId;
        }

        public async System.Threading.Tasks.Task RemoveReactionAsync(long recognitionId, long employeeId, long callerEmployeeId, bool isHrOrAdmin)
        {
            if (!isHrOrAdmin && callerEmployeeId != employeeId)
                throw new ForbiddenException("You can only remove your own reaction.");

            var entity = await _recognitionReactionRepository.GetAsync(recognitionId, employeeId)
                ?? throw new NotFoundException("RecognitionReaction", employeeId);

            _recognitionReactionRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RecognitionReactionResponse>> GetReactionsAsync(long recognitionId)
        {
            var entities = await _recognitionReactionRepository.GetByRecognitionIdAsync(recognitionId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> AddCommentAsync(long recognitionId, RecognitionCommentAddRequest request, long callerEmployeeId)
        {
            if (callerEmployeeId <= 0)
                throw new ForbiddenException("A valid employee identity is required to comment.");

            _ = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var entity = request.ToEntity(recognitionId, callerEmployeeId);
            await _recognitionCommentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CommentId;
        }

        public async Task<List<RecognitionCommentResponse>> GetCommentsAsync(long recognitionId)
        {
            var entities = await _recognitionCommentRepository.GetByRecognitionIdAsync(recognitionId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
