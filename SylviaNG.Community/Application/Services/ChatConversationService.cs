using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class ChatConversationService : IChatConversationService
    {
        private readonly IChatConversationRepository _chatConversationRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFileStorageRepository _fileStorageRepository;
        private readonly INotificationService _notificationService;
        private readonly IMessengerBroadcaster _messengerBroadcaster;
        private readonly IUnitOfWork _unitOfWork;

        public ChatConversationService(
            IChatConversationRepository chatConversationRepository,
            IChatParticipantRepository chatParticipantRepository,
            IChatMessageRepository chatMessageRepository,
            IEmployeeRepository employeeRepository,
            IFileStorageRepository fileStorageRepository,
            INotificationService notificationService,
            IMessengerBroadcaster messengerBroadcaster,
            IUnitOfWork unitOfWork)
        {
            _chatConversationRepository = chatConversationRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _chatMessageRepository = chatMessageRepository;
            _employeeRepository = employeeRepository;
            _fileStorageRepository = fileStorageRepository;
            _notificationService = notificationService;
            _messengerBroadcaster = messengerBroadcaster;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ChatConversationCreateRequest request, long callerEmployeeId)
        {
            var otherEmployeeIds = request.ParticipantEmployeeIds
                .Where(id => id != callerEmployeeId)
                .Distinct()
                .ToList();

            if (request.Type == ConversationTypeEnum.Direct)
            {
                var existing = await _chatConversationRepository.GetDirectConversationAsync(callerEmployeeId, otherEmployeeIds.Single());
                if (existing != null)
                    return existing.ChatConversationId;
            }

            var entity = request.ToEntity(callerEmployeeId);
            await _chatConversationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var now = DateTime.UtcNow;
            var participants = new List<ChatParticipant>
            {
                new ChatParticipant
                {
                    ChatConversationId = entity.ChatConversationId,
                    EmployeeId = callerEmployeeId,
                    IsAdmin = request.Type == ConversationTypeEnum.Group,
                    JoinedAt = now
                }
            };
            participants.AddRange(otherEmployeeIds.Select(id => new ChatParticipant
            {
                ChatConversationId = entity.ChatConversationId,
                EmployeeId = id,
                IsAdmin = false,
                JoinedAt = now
            }));

            await _chatParticipantRepository.AddRangeAsync(participants);
            await _unitOfWork.SaveChangesAsync();

            // US-12.1/US-12.10: every other participant is notified a conversation was
            // started or they were added to a group.
            var creator = await _employeeRepository.GetByIdAsync(callerEmployeeId);
            var creatorName = creator?.EmployeeName ?? "A colleague";
            var title = request.Type == ConversationTypeEnum.Group
                ? $"You were added to \"{entity.Title}\""
                : $"{creatorName} started a conversation with you";

            foreach (var otherEmployeeId in otherEmployeeIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = otherEmployeeId,
                    Title = title,
                    Category = "Messenger",
                    RelatedEntityType = "ChatConversation",
                    RelatedEntityId = entity.ChatConversationId
                });
            }

            // Without this, a brand-new conversation only ever appears in a participant's inbox
            // list after they reload Messenger (the sidebar is otherwise only kept in sync via
            // ConversationUpdated pushes triggered by later messages/mute/pin/etc.) - push one now
            // for every participant, including the creator, so it shows up live immediately.
            foreach (var participant in participants)
            {
                var summary = await GetSummaryForEmployeeAsync(entity.ChatConversationId, participant.EmployeeId);
                await _messengerBroadcaster.BroadcastConversationUpdatedAsync(participant.EmployeeId, summary);
            }

            return entity.ChatConversationId;
        }

        public async Task<ChatConversationResponse> GetByIdAsync(long conversationId, long callerEmployeeId)
        {
            await EnsureActiveParticipantAsync(conversationId, callerEmployeeId);

            var entity = await _chatConversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("ChatConversation", conversationId);

            var participants = await BuildParticipantResponsesAsync(conversationId);
            var response = entity.ToResponse(participants);
            response.GroupAvatarUrl = await ResolveGroupAvatarUrlAsync(entity.GroupAvatarFileId);
            return response;
        }

        /// <summary>
        /// HR/Admin-only: same as GetByIdAsync but deliberately skips EnsureActiveParticipantAsync,
        /// so a moderator reviewing a ChatReport can look up conversation metadata even though they
        /// aren't a participant. Only ever reached via ChatReportController, which is gated by the
        /// HRAdminOnly policy.
        /// </summary>
        public async Task<ChatConversationResponse> GetForModerationAsync(long conversationId)
        {
            var entity = await _chatConversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("ChatConversation", conversationId);

            var participants = await BuildParticipantResponsesAsync(conversationId);
            var response = entity.ToResponse(participants);
            response.GroupAvatarUrl = await ResolveGroupAvatarUrlAsync(entity.GroupAvatarFileId);
            return response;
        }

        public async Task<PagedResult<ChatConversationSummaryResponse>> GetMyConversationsPagedAsync(long callerEmployeeId, PagedRequest request)
        {
            var pagedResult = await _chatConversationRepository.GetMyConversationsPagedAsync(callerEmployeeId, request);

            var items = new List<ChatConversationSummaryResponse>();
            foreach (var conversation in pagedResult.Data)
            {
                items.Add(await BuildSummaryAsync(conversation, callerEmployeeId));
            }

            return new PagedResult<ChatConversationSummaryResponse>
            {
                Data = items,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<ChatConversationSummaryResponse> GetSummaryForEmployeeAsync(long conversationId, long employeeId)
        {
            var conversation = await _chatConversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("ChatConversation", conversationId);

            return await BuildSummaryAsync(conversation, employeeId);
        }

        public async Task<bool> IsActiveParticipantAsync(long conversationId, long employeeId)
        {
            return await _chatParticipantRepository.IsActiveParticipantAsync(conversationId, employeeId);
        }

        public async Task MarkReadAsync(long conversationId, long callerEmployeeId)
        {
            var participant = await _chatParticipantRepository.GetActiveAsync(conversationId, callerEmployeeId)
                ?? throw new ForbiddenException("You are not a participant of this conversation.");

            var now = DateTime.UtcNow;
            participant.LastReadAt = now;
            _chatParticipantRepository.Update(participant);
            await _unitOfWork.SaveChangesAsync();

            await _messengerBroadcaster.BroadcastMessageReadAsync(conversationId, callerEmployeeId, now);

            var summary = await GetSummaryForEmployeeAsync(conversationId, callerEmployeeId);
            await _messengerBroadcaster.BroadcastConversationUpdatedAsync(callerEmployeeId, summary);
        }

        public async Task SetMutedAsync(long conversationId, long callerEmployeeId, bool isMuted)
        {
            var participant = await _chatParticipantRepository.GetActiveAsync(conversationId, callerEmployeeId)
                ?? throw new ForbiddenException("You are not a participant of this conversation.");

            participant.IsMuted = isMuted;
            _chatParticipantRepository.Update(participant);
            await _unitOfWork.SaveChangesAsync();

            var summary = await GetSummaryForEmployeeAsync(conversationId, callerEmployeeId);
            await _messengerBroadcaster.BroadcastConversationUpdatedAsync(callerEmployeeId, summary);
        }

        public async Task SetPinnedAsync(long conversationId, long callerEmployeeId, bool isPinned)
        {
            var participant = await _chatParticipantRepository.GetActiveAsync(conversationId, callerEmployeeId)
                ?? throw new ForbiddenException("You are not a participant of this conversation.");

            participant.IsPinned = isPinned;
            participant.PinnedAt = isPinned ? DateTime.UtcNow : null;
            _chatParticipantRepository.Update(participant);
            await _unitOfWork.SaveChangesAsync();

            var summary = await GetSummaryForEmployeeAsync(conversationId, callerEmployeeId);
            await _messengerBroadcaster.BroadcastConversationUpdatedAsync(callerEmployeeId, summary);
        }

        public async Task UpdateGroupAsync(long conversationId, ChatConversationUpdateGroupRequest request, long callerEmployeeId)
        {
            var participant = await _chatParticipantRepository.GetActiveAsync(conversationId, callerEmployeeId)
                ?? throw new ForbiddenException("You are not a participant of this conversation.");

            var entity = await _chatConversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("ChatConversation", conversationId);

            if (entity.Type != ConversationTypeEnum.Group)
                throw new FluentValidation.ValidationException("Only group conversations can have a title or photo.");

            if (!participant.IsAdmin)
                throw new ForbiddenException("Only a group admin can update this conversation.");

            if (request.Title != null)
                entity.Title = request.Title.Trim();

            if (request.GroupAvatarFileId != null)
            {
                var file = await _fileStorageRepository.GetByIdAsync(request.GroupAvatarFileId.Value)
                    ?? throw new NotFoundException("FileStorage", request.GroupAvatarFileId.Value);
                entity.GroupAvatarFileId = file.FileId;
            }

            _chatConversationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var participants = await BuildParticipantResponsesAsync(conversationId);
            var response = entity.ToResponse(participants);
            response.GroupAvatarUrl = await ResolveGroupAvatarUrlAsync(entity.GroupAvatarFileId);
            await _messengerBroadcaster.BroadcastGroupUpdatedAsync(conversationId, response);

            // Title/photo also surface in every participant's inbox row (DisplayName/AvatarUrl for a Group), not just the open thread.
            var allParticipants = await _chatParticipantRepository.GetActiveByConversationIdAsync(conversationId);
            foreach (var otherParticipant in allParticipants)
            {
                var summary = await GetSummaryForEmployeeAsync(conversationId, otherParticipant.EmployeeId);
                await _messengerBroadcaster.BroadcastConversationUpdatedAsync(otherParticipant.EmployeeId, summary);
            }
        }

        private async Task<string?> ResolveGroupAvatarUrlAsync(long? groupAvatarFileId)
        {
            if (groupAvatarFileId == null) return null;

            var file = await _fileStorageRepository.GetByIdAsync(groupAvatarFileId.Value);
            return file?.StoragePath;
        }

        private async Task EnsureActiveParticipantAsync(long conversationId, long employeeId)
        {
            var isParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(conversationId, employeeId);
            if (!isParticipant)
                throw new ForbiddenException("You are not a participant of this conversation.");
        }

        private async Task<List<ChatParticipantResponse>> BuildParticipantResponsesAsync(long conversationId)
        {
            var participants = await _chatParticipantRepository.GetActiveByConversationIdAsync(conversationId);

            var result = new List<ChatParticipantResponse>();
            foreach (var participant in participants)
            {
                var employee = await _employeeRepository.GetByIdAsync(participant.EmployeeId);
                result.Add(new ChatParticipantResponse
                {
                    ChatParticipantId = participant.ChatParticipantId,
                    EmployeeId = participant.EmployeeId,
                    EmployeeName = employee?.EmployeeName ?? "Unknown",
                    EmployeePhotoUrl = employee?.PhotoUrl,
                    IsAdmin = participant.IsAdmin,
                    JoinedAt = participant.JoinedAt,
                    LastReadAt = participant.LastReadAt,
                    IsMuted = participant.IsMuted,
                    IsPinned = participant.IsPinned
                });
            }
            return result;
        }

        private async Task<ChatConversationSummaryResponse> BuildSummaryAsync(ChatConversation conversation, long callerEmployeeId)
        {
            var myParticipant = await _chatParticipantRepository.GetActiveAsync(conversation.ChatConversationId, callerEmployeeId);

            var displayName = conversation.Title ?? string.Empty;
            long? otherEmployeeId = null;
            string? avatarUrl = null;

            if (conversation.Type == ConversationTypeEnum.Direct)
            {
                var participants = await _chatParticipantRepository.GetActiveByConversationIdAsync(conversation.ChatConversationId);
                var other = participants.FirstOrDefault(p => p.EmployeeId != callerEmployeeId);
                if (other != null)
                {
                    otherEmployeeId = other.EmployeeId;
                    var employee = await _employeeRepository.GetByIdAsync(other.EmployeeId);
                    displayName = employee?.EmployeeName ?? "Unknown";
                    avatarUrl = employee?.PhotoUrl;
                }
            }
            else
            {
                avatarUrl = await ResolveGroupAvatarUrlAsync(conversation.GroupAvatarFileId);
            }

            var unreadCount = await _chatMessageRepository.GetUnreadCountAsync(conversation.ChatConversationId, callerEmployeeId, myParticipant?.LastReadAt);

            return new ChatConversationSummaryResponse
            {
                ChatConversationId = conversation.ChatConversationId,
                Type = conversation.Type,
                DisplayName = displayName,
                AvatarUrl = avatarUrl,
                OtherEmployeeId = otherEmployeeId,
                LastMessageAt = conversation.LastMessageAt,
                LastMessagePreview = conversation.LastMessagePreview,
                UnreadCount = unreadCount,
                IsMuted = myParticipant?.IsMuted ?? false,
                IsPinned = myParticipant?.IsPinned ?? false
            };
        }
    }
}
