using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
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
        private readonly IRecognitionBadgeRepository _recognitionBadgeRepository;
        private readonly IRecognitionReactionRepository _recognitionReactionRepository;
        private readonly IRecognitionCommentRepository _recognitionCommentRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public RecognitionService(
            IRecognitionRepository recognitionRepository,
            IRecognitionBadgeRepository recognitionBadgeRepository,
            IRecognitionReactionRepository recognitionReactionRepository,
            IRecognitionCommentRepository recognitionCommentRepository,
            IBadgeRepository badgeRepository,
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _recognitionRepository = recognitionRepository;
            _recognitionBadgeRepository = recognitionBadgeRepository;
            _recognitionReactionRepository = recognitionReactionRepository;
            _recognitionCommentRepository = recognitionCommentRepository;
            _badgeRepository = badgeRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        private async Task<string> GetEmployeeNameAsync(long employeeId) =>
            (await _employeeRepository.GetByIdAsync(employeeId))?.EmployeeName ?? "Someone";

        public async Task<long> CreateAsync(RecognitionCreateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            if (callerEmployeeId <= 0)
                throw new ForbiddenException("A valid employee identity is required to send a recognition.");

            if (request.IsHrIssued && !isHrOrAdmin)
                throw new ForbiddenException("Only HR/Admin can issue a formal award.");

            var badgeIds = request.BadgeIds.Distinct().ToList();
            if (badgeIds.Count > 0)
            {
                var existingBadges = await _badgeRepository.FindAsync(b => badgeIds.Contains(b.BadgeId));
                var missingBadgeId = badgeIds.FirstOrDefault(id => !existingBadges.Any(b => b.BadgeId == id));
                if (missingBadgeId != default)
                    throw new NotFoundException("Badge", missingBadgeId);
            }

            var entity = request.ToEntity(callerEmployeeId);
            await _recognitionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            if (badgeIds.Count > 0)
            {
                var recognitionBadges = badgeIds.Select(badgeId => new RecognitionBadge
                {
                    RecognitionId = entity.RecognitionId,
                    BadgeId = badgeId
                });
                await _recognitionBadgeRepository.AddRangeAsync(recognitionBadges);
                await _unitOfWork.SaveChangesAsync();
            }

            if (entity.RecipientId != callerEmployeeId)
            {
                var senderName = await GetEmployeeNameAsync(callerEmployeeId);
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = entity.RecipientId,
                    Title = $"{senderName} recognized you",
                    Category = "Recognition",
                    RelatedEntityType = "Recognition",
                    RelatedEntityId = entity.RecognitionId
                });
            }

            return entity.RecognitionId;
        }

        public async Task<RecognitionResponse> GetByIdAsync(long recognitionId)
        {
            var entity = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var recognitionBadges = await _recognitionBadgeRepository.GetByRecognitionIdAsync(recognitionId);
            var badgeIds = recognitionBadges.Select(rb => rb.BadgeId).Distinct().ToList();
            var badges = badgeIds.Count == 0
                ? Enumerable.Empty<Badge>()
                : await _badgeRepository.FindAsync(b => badgeIds.Contains(b.BadgeId));

            return entity.ToResponse(badges);
        }

        public async Task<PagedResult<RecognitionResponse>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null)
        {
            var pagedResult = await _recognitionRepository.GetPaginatedAsync(request, senderId, recipientId);

            var recognitionIds = pagedResult.Data.Select(e => e.RecognitionId).ToList();
            var recognitionBadges = recognitionIds.Count == 0
                ? new List<RecognitionBadge>()
                : await _recognitionBadgeRepository.GetByRecognitionIdsAsync(recognitionIds);

            var badgeIds = recognitionBadges.Select(rb => rb.BadgeId).Distinct().ToList();
            var badgesById = badgeIds.Count == 0
                ? new Dictionary<long, Badge>()
                : (await _badgeRepository.FindAsync(b => badgeIds.Contains(b.BadgeId)))
                    .ToDictionary(b => b.BadgeId);

            var badgesByRecognitionId = recognitionBadges
                .GroupBy(rb => rb.RecognitionId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Where(rb => badgesById.ContainsKey(rb.BadgeId)).Select(rb => badgesById[rb.BadgeId]));

            return new PagedResult<RecognitionResponse>
            {
                Data = pagedResult.Data
                    .Select(e => e.ToResponse(badgesByRecognitionId.TryGetValue(e.RecognitionId, out var badges) ? badges : null))
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

            var recognition = await _recognitionRepository.GetByIdAsync(recognitionId)
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

            await NotifyRecognitionStakeholdersAsync(recognition, callerEmployeeId, "reacted to a recognition", "RecognitionReaction");

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

            var recognition = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var entity = request.ToEntity(recognitionId, callerEmployeeId);
            await _recognitionCommentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await NotifyRecognitionStakeholdersAsync(recognition, callerEmployeeId, "commented on a recognition", "RecognitionComment");

            return entity.CommentId;
        }

        /// <summary>Notifies the recognition's sender and recipient (whichever isn't the acting employee) about a comment/reaction on it.</summary>
        private async System.Threading.Tasks.Task NotifyRecognitionStakeholdersAsync(Recognition recognition, long actingEmployeeId, string action, string category)
        {
            var actorName = await GetEmployeeNameAsync(actingEmployeeId);
            var stakeholderIds = new[] { recognition.SenderId, recognition.RecipientId }
                .Distinct()
                .Where(id => id != actingEmployeeId);

            foreach (var stakeholderId in stakeholderIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = stakeholderId,
                    Title = $"{actorName} {action}",
                    Category = category,
                    RelatedEntityType = "Recognition",
                    RelatedEntityId = recognition.RecognitionId
                });
            }
        }

        public async Task<List<RecognitionCommentResponse>> GetCommentsAsync(long recognitionId)
        {
            var entities = await _recognitionCommentRepository.GetByRecognitionIdAsync(recognitionId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
