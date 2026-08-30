using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IChatMessageService
    {
        Task<ChatMessageResponse> SendAsync(long conversationId, ChatMessageSendRequest request, long callerEmployeeId);
        Task<PagedResult<ChatMessageResponse>> GetPagedAsync(long conversationId, long callerEmployeeId, PagedRequest request);

        /// <summary>HR/Admin-only: reads the full thread of a conversation without a participant check (see ChatMessageService).</summary>
        Task<PagedResult<ChatMessageResponse>> GetPagedForModerationAsync(long conversationId, PagedRequest request);

        /// <summary>Body-text search scoped to conversations the caller is an active participant of (US-12.2).</summary>
        Task<PagedResult<ChatMessageResponse>> SearchAsync(long callerEmployeeId, string searchTerm, PagedRequest request);

        /// <summary>Adds a reaction; reacting again with the same type toggles it off (returns null); a different type switches it (US-12.8).</summary>
        Task<ChatMessageReactionResponse?> ReactAsync(long chatMessageId, ReactionTypeEnum reactionType, long callerEmployeeId);

        /// <summary>"Remove for Everyone" - sender-only. Keeps the row as a tombstone rather than hard-deleting it.</summary>
        Task DeleteAsync(long chatMessageId, long callerEmployeeId);

        /// <summary>Copies this message's content into one or more of the caller's own other conversations. Silently skips any target the caller isn't an active participant of.</summary>
        Task ForwardAsync(long chatMessageId, List<long> targetConversationIds, long callerEmployeeId);

        /// <summary>Files a moderation report against a specific message.</summary>
        Task ReportAsync(long chatMessageId, string reason, long callerEmployeeId);
    }
}
