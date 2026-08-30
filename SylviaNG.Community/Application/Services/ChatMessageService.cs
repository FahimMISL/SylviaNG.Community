using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
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
    public class ChatMessageService : IChatMessageService
    {
        private const int PreviewMaxLength = 120;

        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatMessageAttachmentRepository _chatMessageAttachmentRepository;
        private readonly IChatMessageReactionRepository _chatMessageReactionRepository;
        private readonly IChatConversationRepository _chatConversationRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFileStorageRepository _fileStorageRepository;
        private readonly IChatReportRepository _chatReportRepository;
        private readonly INotificationService _notificationService;
        private readonly IEmployeeKeycloakAccountRepository _employeeKeycloakAccountRepository;
        private readonly IChatConversationService _chatConversationService;
        private readonly IMessengerBroadcaster _messengerBroadcaster;
        private readonly IUnitOfWork _unitOfWork;

        public ChatMessageService(
            IChatMessageRepository chatMessageRepository,
            IChatMessageAttachmentRepository chatMessageAttachmentRepository,
            IChatMessageReactionRepository chatMessageReactionRepository,
            IChatConversationRepository chatConversationRepository,
            IChatParticipantRepository chatParticipantRepository,
            IEmployeeRepository employeeRepository,
            IFileStorageRepository fileStorageRepository,
            IChatReportRepository chatReportRepository,
            INotificationService notificationService,
            IEmployeeKeycloakAccountRepository employeeKeycloakAccountRepository,
            IChatConversationService chatConversationService,
            IMessengerBroadcaster messengerBroadcaster,
            IUnitOfWork unitOfWork)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatMessageAttachmentRepository = chatMessageAttachmentRepository;
            _chatMessageReactionRepository = chatMessageReactionRepository;
            _chatConversationRepository = chatConversationRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _employeeRepository = employeeRepository;
            _fileStorageRepository = fileStorageRepository;
            _chatReportRepository = chatReportRepository;
            _notificationService = notificationService;
            _employeeKeycloakAccountRepository = employeeKeycloakAccountRepository;
            _chatConversationService = chatConversationService;
            _messengerBroadcaster = messengerBroadcaster;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Broadcasts a notification to every employee with an active HR/Admin Keycloak account
        /// (see EmployeeKeycloakAccount.AssignedRole - mirrors MarketplaceService.NotifyModeratorsAsync,
        /// the only queryable "who are the moderators" source in this codebase). Sends nothing,
        /// silently, if no one currently holds HR/Admin access.
        /// </summary>
        private async Task NotifyModeratorsAsync(string title, string message, long chatReportId)
        {
            var moderatorIds = await _employeeKeycloakAccountRepository.GetEmployeeIdsByRolesAsync(new[] { "HR", "Admin" });
            foreach (var moderatorId in moderatorIds)
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = moderatorId,
                    Title = title,
                    Message = message,
                    Category = "ChatModeration",
                    RelatedEntityType = "ChatReport",
                    RelatedEntityId = chatReportId
                });
            }
        }

        public async Task<ChatMessageResponse> SendAsync(long conversationId, ChatMessageSendRequest request, long callerEmployeeId)
        {
            var isParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(conversationId, callerEmployeeId);
            if (!isParticipant)
                throw new ForbiddenException("You are not a participant of this conversation.");

            var conversation = await _chatConversationRepository.GetByIdAsync(conversationId)
                ?? throw new NotFoundException("ChatConversation", conversationId);

            ChatMessage? replyTarget = null;
            if (request.ReplyToMessageId != null)
            {
                replyTarget = await _chatMessageRepository.GetByIdAsync(request.ReplyToMessageId.Value);
                if (replyTarget == null || replyTarget.ChatConversationId != conversationId)
                    throw new NotFoundException("ChatMessage", request.ReplyToMessageId.Value);
            }

            // Attachments referenced files must already exist (uploaded via community/file-upload
            // beforehand) - validated and fetched up front so the same FileStorage rows can be
            // reused below to build the response without a second round of lookups.
            var attachmentFiles = new Dictionary<long, FileStorage>();
            foreach (var attachmentRequest in request.Attachments)
            {
                var file = await _fileStorageRepository.GetByIdAsync(attachmentRequest.FileStorageId)
                    ?? throw new NotFoundException("FileStorage", attachmentRequest.FileStorageId);
                attachmentFiles[attachmentRequest.FileStorageId] = file;
            }

            var now = DateTime.UtcNow;
            var entity = request.ToEntity(conversationId, callerEmployeeId, now);
            await _chatMessageRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var attachmentEntities = request.Attachments.Select(a => a.ToEntity(entity.ChatMessageId)).ToList();
            if (attachmentEntities.Count > 0)
            {
                await _chatMessageAttachmentRepository.AddRangeAsync(attachmentEntities);
            }

            conversation.LastMessageAt = now;
            conversation.LastMessagePreview = BuildPreview(entity, attachmentEntities);
            _chatConversationRepository.Update(conversation);

            await _unitOfWork.SaveChangesAsync();

            var sender = await _employeeRepository.GetByIdAsync(callerEmployeeId);
            var senderName = sender?.EmployeeName ?? "Unknown";
            var attachmentResponses = attachmentEntities
                .Select(a => a.ToResponse(attachmentFiles[a.FileStorageId]))
                .ToList();

            ChatMessageReplyPreviewResponse? replyPreview = null;
            if (replyTarget != null)
            {
                var replySender = await _employeeRepository.GetByIdAsync(replyTarget.SenderEmployeeId);
                var replyHasAttachment = await _chatMessageAttachmentRepository.GetByMessageIdsAsync(new List<long> { replyTarget.ChatMessageId });
                replyPreview = replyTarget.ToReplyPreview(replySender?.EmployeeName ?? "Unknown", replyHasAttachment.Count > 0);
            }

            var response = entity.ToResponse(senderName, attachmentResponses, replyTo: replyPreview);
            response.SenderPhotoUrl = sender?.PhotoUrl;

            await _messengerBroadcaster.BroadcastMessageAsync(conversationId, response);

            var allParticipants = await _chatParticipantRepository.GetActiveByConversationIdAsync(conversationId);

            foreach (var participant in allParticipants)
            {
                if (participant.EmployeeId != callerEmployeeId && !participant.IsMuted)
                {
                    // US-12.3/US-12.13: every message notifies every other active participant,
                    // every time, unless that participant has muted this conversation - muting
                    // only silences the Notification Center entry, not the live message itself
                    // (still delivered via BroadcastMessageAsync/ConversationUpdated below).
                    await _notificationService.CreateAsync(new NotificationCreateRequest
                    {
                        EmployeeId = participant.EmployeeId,
                        Title = $"New message from {senderName}",
                        Message = conversation.LastMessagePreview,
                        Category = "Messenger",
                        RelatedEntityType = "ChatConversation",
                        RelatedEntityId = conversationId
                    });
                }

                var summary = await _chatConversationService.GetSummaryForEmployeeAsync(conversationId, participant.EmployeeId);
                await _messengerBroadcaster.BroadcastConversationUpdatedAsync(participant.EmployeeId, summary);
            }

            return response;
        }

        public async Task<PagedResult<ChatMessageResponse>> GetPagedAsync(long conversationId, long callerEmployeeId, PagedRequest request)
        {
            var isParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(conversationId, callerEmployeeId);
            if (!isParticipant)
                throw new ForbiddenException("You are not a participant of this conversation.");

            var pagedResult = await _chatMessageRepository.GetByConversationPagedAsync(conversationId, request);
            return await BuildResponsePageAsync(pagedResult);
        }

        /// <summary>
        /// HR/Admin-only: same as GetPagedAsync but deliberately skips the participant check, so a
        /// moderator reviewing a ChatReport can read the full surrounding thread even though they
        /// aren't a participant of the conversation. Only ever reached via ChatReportController,
        /// which is gated by the HRAdminOnly policy.
        /// </summary>
        public async Task<PagedResult<ChatMessageResponse>> GetPagedForModerationAsync(long conversationId, PagedRequest request)
        {
            var pagedResult = await _chatMessageRepository.GetByConversationPagedAsync(conversationId, request);
            return await BuildResponsePageAsync(pagedResult);
        }

        public async Task<PagedResult<ChatMessageResponse>> SearchAsync(long callerEmployeeId, string searchTerm, PagedRequest request)
        {
            var pagedResult = await _chatMessageRepository.SearchAsync(callerEmployeeId, searchTerm, request);
            return await BuildResponsePageAsync(pagedResult);
        }

        public async Task<ChatMessageReactionResponse?> ReactAsync(long chatMessageId, ReactionTypeEnum reactionType, long callerEmployeeId)
        {
            var message = await _chatMessageRepository.GetByIdAsync(chatMessageId)
                ?? throw new NotFoundException("ChatMessage", chatMessageId);

            var isParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(message.ChatConversationId, callerEmployeeId);
            if (!isParticipant)
                throw new ForbiddenException("You are not a participant of this conversation.");

            var existing = await _chatMessageReactionRepository.GetAsync(chatMessageId, callerEmployeeId);

            ChatMessageReaction? resultEntity;
            if (existing == null)
            {
                resultEntity = new ChatMessageReaction { ChatMessageId = chatMessageId, EmployeeId = callerEmployeeId, ReactionType = reactionType };
                await _chatMessageReactionRepository.AddAsync(resultEntity);
                await _unitOfWork.SaveChangesAsync();

                if (message.SenderEmployeeId != callerEmployeeId)
                {
                    var reactor = await _employeeRepository.GetByIdAsync(callerEmployeeId);
                    await _notificationService.CreateAsync(new NotificationCreateRequest
                    {
                        EmployeeId = message.SenderEmployeeId,
                        Title = $"{reactor?.EmployeeName ?? "Someone"} reacted to your message",
                        Category = "Messenger",
                        RelatedEntityType = "ChatConversation",
                        RelatedEntityId = message.ChatConversationId
                    });
                }
            }
            else if (existing.ReactionType == reactionType)
            {
                // Toggle off - reacting again with the same type removes it, matching PostReaction's precedent.
                _chatMessageReactionRepository.Delete(existing);
                await _unitOfWork.SaveChangesAsync();
                resultEntity = null;
            }
            else
            {
                existing.ReactionType = reactionType;
                _chatMessageReactionRepository.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                resultEntity = existing;
            }

            await _messengerBroadcaster.BroadcastMessageReactedAsync(message.ChatConversationId, chatMessageId, callerEmployeeId, resultEntity?.ReactionType);

            return resultEntity?.ToResponse(message.ChatConversationId);
        }

        public async Task DeleteAsync(long chatMessageId, long callerEmployeeId)
        {
            var message = await _chatMessageRepository.GetByIdAsync(chatMessageId)
                ?? throw new NotFoundException("ChatMessage", chatMessageId);

            if (message.SenderEmployeeId != callerEmployeeId)
                throw new ForbiddenException("You can only remove your own messages.");

            if (message.IsDeleted) return;

            message.IsDeleted = true;
            _chatMessageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            await _messengerBroadcaster.BroadcastMessageDeletedAsync(message.ChatConversationId, chatMessageId);
        }

        public async Task ForwardAsync(long chatMessageId, List<long> targetConversationIds, long callerEmployeeId)
        {
            var source = await _chatMessageRepository.GetByIdAsync(chatMessageId)
                ?? throw new NotFoundException("ChatMessage", chatMessageId);

            if (source.IsDeleted)
                throw new NotFoundException("ChatMessage", chatMessageId);

            var isSourceParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(source.ChatConversationId, callerEmployeeId);
            if (!isSourceParticipant)
                throw new ForbiddenException("You are not a participant of this conversation.");

            var sourceAttachments = await _chatMessageAttachmentRepository.GetByMessageIdsAsync(new List<long> { chatMessageId });
            var sender = await _employeeRepository.GetByIdAsync(callerEmployeeId);
            var senderName = sender?.EmployeeName ?? "Unknown";

            var files = new Dictionary<long, FileStorage>();
            foreach (var attachment in sourceAttachments)
            {
                if (files.ContainsKey(attachment.FileStorageId)) continue;
                var file = await _fileStorageRepository.GetByIdAsync(attachment.FileStorageId);
                if (file != null) files[attachment.FileStorageId] = file;
            }

            foreach (var targetConversationId in targetConversationIds.Distinct())
            {
                var isTargetParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(targetConversationId, callerEmployeeId);
                if (!isTargetParticipant) continue; // Can only forward into conversations the caller is themself a member of.

                var conversation = await _chatConversationRepository.GetByIdAsync(targetConversationId);
                if (conversation == null) continue;

                var now = DateTime.UtcNow;
                var newMessage = new ChatMessage
                {
                    ChatConversationId = targetConversationId,
                    SenderEmployeeId = callerEmployeeId,
                    Body = source.Body,
                    MessageType = source.MessageType,
                    IsForwarded = true,
                    SentAt = now
                };
                await _chatMessageRepository.AddAsync(newMessage);
                await _unitOfWork.SaveChangesAsync();

                var newAttachmentEntities = sourceAttachments
                    .Where(a => files.ContainsKey(a.FileStorageId))
                    .Select(a => new ChatMessageAttachment
                    {
                        ChatMessageId = newMessage.ChatMessageId,
                        FileStorageId = a.FileStorageId,
                        AttachmentType = a.AttachmentType,
                        DurationSeconds = a.DurationSeconds
                    })
                    .ToList();
                if (newAttachmentEntities.Count > 0)
                {
                    await _chatMessageAttachmentRepository.AddRangeAsync(newAttachmentEntities);
                }

                conversation.LastMessageAt = now;
                conversation.LastMessagePreview = BuildPreview(newMessage, newAttachmentEntities);
                _chatConversationRepository.Update(conversation);
                await _unitOfWork.SaveChangesAsync();

                var attachmentResponses = newAttachmentEntities.Select(a => a.ToResponse(files[a.FileStorageId])).ToList();
                var response = newMessage.ToResponse(senderName, attachmentResponses);
                await _messengerBroadcaster.BroadcastMessageAsync(targetConversationId, response);

                var targetParticipants = await _chatParticipantRepository.GetActiveByConversationIdAsync(targetConversationId);
                foreach (var participant in targetParticipants)
                {
                    var summary = await _chatConversationService.GetSummaryForEmployeeAsync(targetConversationId, participant.EmployeeId);
                    await _messengerBroadcaster.BroadcastConversationUpdatedAsync(participant.EmployeeId, summary);
                }
            }
        }

        public async Task ReportAsync(long chatMessageId, string reason, long callerEmployeeId)
        {
            var message = await _chatMessageRepository.GetByIdAsync(chatMessageId)
                ?? throw new NotFoundException("ChatMessage", chatMessageId);

            var isParticipant = await _chatParticipantRepository.IsActiveParticipantAsync(message.ChatConversationId, callerEmployeeId);
            if (!isParticipant)
                throw new ForbiddenException("You are not a participant of this conversation.");

            var report = new ChatReport
            {
                ReportedByEmployeeId = callerEmployeeId,
                ChatConversationId = message.ChatConversationId,
                ChatMessageId = chatMessageId,
                Reason = reason,
                Status = "Pending"
            };
            await _chatReportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync();

            var reporterName = (await _employeeRepository.GetByIdAsync(callerEmployeeId))?.EmployeeName ?? "Someone";
            await NotifyModeratorsAsync("A chat message was reported", $"{reporterName} reported a message: {reason}", report.ChatReportId);
        }

        private async Task<PagedResult<ChatMessageResponse>> BuildResponsePageAsync(PagedResult<ChatMessage> pagedResult)
        {
            var messageIds = pagedResult.Data.Select(m => m.ChatMessageId).ToList();
            var attachments = messageIds.Count > 0
                ? await _chatMessageAttachmentRepository.GetByMessageIdsAsync(messageIds)
                : new List<ChatMessageAttachment>();
            var reactions = messageIds.Count > 0
                ? await _chatMessageReactionRepository.GetByMessageIdsAsync(messageIds)
                : new List<ChatMessageReaction>();

            var files = new Dictionary<long, FileStorage>();
            foreach (var attachment in attachments)
            {
                if (files.ContainsKey(attachment.FileStorageId)) continue;

                var file = await _fileStorageRepository.GetByIdAsync(attachment.FileStorageId);
                if (file != null) files[attachment.FileStorageId] = file;
            }

            // Batch-resolve reply-target previews (quoted snippet + sender name) so each reply
            // bubble doesn't need a second round trip to fetch the message it's replying to.
            var replyTargetIds = pagedResult.Data.Where(m => m.ReplyToMessageId != null).Select(m => m.ReplyToMessageId!.Value).Distinct().ToList();
            var replyPreviews = new Dictionary<long, ChatMessageReplyPreviewResponse>();
            foreach (var replyTargetId in replyTargetIds)
            {
                var replyTarget = await _chatMessageRepository.GetByIdAsync(replyTargetId);
                if (replyTarget == null) continue;

                var replySender = await _employeeRepository.GetByIdAsync(replyTarget.SenderEmployeeId);
                var replyHasAttachment = await _chatMessageAttachmentRepository.GetByMessageIdsAsync(new List<long> { replyTargetId });
                replyPreviews[replyTargetId] = replyTarget.ToReplyPreview(replySender?.EmployeeName ?? "Unknown", replyHasAttachment.Count > 0);
            }

            var items = new List<ChatMessageResponse>();
            foreach (var message in pagedResult.Data)
            {
                var sender = await _employeeRepository.GetByIdAsync(message.SenderEmployeeId);
                var messageAttachments = attachments
                    .Where(a => a.ChatMessageId == message.ChatMessageId && files.ContainsKey(a.FileStorageId))
                    .Select(a => a.ToResponse(files[a.FileStorageId]))
                    .ToList();
                var messageReactions = reactions
                    .Where(r => r.ChatMessageId == message.ChatMessageId)
                    .Select(r => r.ToResponse(message.ChatConversationId))
                    .ToList();
                var replyTo = message.ReplyToMessageId != null && replyPreviews.TryGetValue(message.ReplyToMessageId.Value, out var preview)
                    ? preview
                    : null;

                var messageResponse = message.ToResponse(sender?.EmployeeName ?? "Unknown", messageAttachments, messageReactions, replyTo);
                messageResponse.SenderPhotoUrl = sender?.PhotoUrl;
                items.Add(messageResponse);
            }

            return new PagedResult<ChatMessageResponse>
            {
                Data = items,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        private static string? BuildPreview(ChatMessage message, List<ChatMessageAttachment> attachments)
        {
            if (!string.IsNullOrEmpty(message.Body))
                return Truncate(message.Body);

            if (attachments.Count == 0)
                return null;

            return message.MessageType switch
            {
                MessageTypeEnum.Voice => "\U0001F3A4 Voice message",
                _ => attachments.Count == 1 && attachments[0].AttachmentType == ChatAttachmentTypeEnum.Image
                    ? "\U0001F4F7 Photo"
                    : "\U0001F4CE Attachment"
            };
        }

        private static string? Truncate(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= PreviewMaxLength ? value : value[..PreviewMaxLength] + "...";
        }
    }
}
