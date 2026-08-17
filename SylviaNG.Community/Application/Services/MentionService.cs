using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class MentionService : IMentionService
    {
        private readonly IMentionRepository _mentionRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public MentionService(
            IMentionRepository mentionRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _mentionRepository = mentionRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<long> CreateAsync(MentionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _mentionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var mentionedByName = (await _employeeRepository.GetByIdAsync(entity.MentionedBy))?.EmployeeName ?? "Someone";

            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = entity.MentionedEmployeeId,
                Title = $"{mentionedByName} mentioned you",
                Category = entity.EntityType == "PostComment" ? "CommentMention" : "Mention",
                RelatedEntityType = entity.EntityType,
                RelatedEntityId = entity.EntityId
            });

            return entity.MentionId;
        }

        public async Task CreateMentionsAsync(string entityType, long entityId, long authorEmployeeId, IEnumerable<long>? mentionedEmployeeIds)
        {
            if (mentionedEmployeeIds == null)
                return;

            var uniqueIds = mentionedEmployeeIds.Distinct().Where(id => id != authorEmployeeId);

            foreach (var mentionedEmployeeId in uniqueIds)
            {
                await CreateAsync(new MentionCreateRequest
                {
                    MentionedEmployeeId = mentionedEmployeeId,
                    MentionedBy = authorEmployeeId,
                    EntityType = entityType,
                    EntityId = entityId
                });
            }
        }

        public async Task<PagedResult<MentionResponse>> GetPaginatedForEmployeeAsync(long mentionedEmployeeId, PagedRequest request)
        {
            var pagedResult = await _mentionRepository.GetPaginatedForEmployeeAsync(mentionedEmployeeId, request);

            return new PagedResult<MentionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<List<MentionResponse>> GetByEntityAsync(string entityType, long entityId)
        {
            var entities = await _mentionRepository.GetByEntityAsync(entityType, entityId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
