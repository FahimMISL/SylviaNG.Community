using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IChatConversationService
    {
        Task<long> CreateAsync(ChatConversationCreateRequest request, long callerEmployeeId);
        Task<ChatConversationResponse> GetByIdAsync(long conversationId, long callerEmployeeId);

        /// <summary>HR/Admin-only: same as GetByIdAsync but skips the participant check (see ChatConversationService).</summary>
        Task<ChatConversationResponse> GetForModerationAsync(long conversationId);
        Task<PagedResult<ChatConversationSummaryResponse>> GetMyConversationsPagedAsync(long callerEmployeeId, PagedRequest request);

        /// <summary>
        /// One conversation's summary from a single employee's point of view (unread count,
        /// mute/pin state). Used by ChatMessageService to build the per-recipient
        /// ConversationUpdated push after a new message, without duplicating the
        /// summary-building logic across services.
        /// </summary>
        Task<ChatConversationSummaryResponse> GetSummaryForEmployeeAsync(long conversationId, long employeeId);

        /// <summary>
        /// Used by MessengerHub to authorize JoinConversation before adding a connection to
        /// the conversation's SignalR group - a second, independent entry point into the
        /// data that needs its own check, not just the REST controller's.
        /// </summary>
        Task<bool> IsActiveParticipantAsync(long conversationId, long employeeId);

        /// <summary>Advances the caller's own read watermark to now (US-12.7).</summary>
        Task MarkReadAsync(long conversationId, long callerEmployeeId);

        /// <summary>Mutes/unmutes the caller's own Notification Center entries for this conversation (US-12.13) - does not affect the message list or other participants.</summary>
        Task SetMutedAsync(long conversationId, long callerEmployeeId, bool isMuted);

        /// <summary>Pins/unpins this conversation to the top of the caller's own inbox (US-12.13).</summary>
        Task SetPinnedAsync(long conversationId, long callerEmployeeId, bool isPinned);

        /// <summary>Admin-only: updates a group conversation's title and/or photo. Throws if the conversation isn't a Group, or the caller isn't an admin participant.</summary>
        Task UpdateGroupAsync(long conversationId, ChatConversationUpdateGroupRequest request, long callerEmployeeId);
    }
}
