using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    /// <summary>
    /// Pushes Messenger events over MessengerHub. Mirrors INotificationBroadcaster/
    /// IFeedBroadcaster's placement (Application defines the contract, Infrastructure
    /// implements it with SignalR, since Application can't depend on AspNetCore.SignalR).
    /// </summary>
    public interface IMessengerBroadcaster
    {
        /// <summary>Group-targeted: everyone who currently has this conversation open.</summary>
        Task BroadcastMessageAsync(long conversationId, ChatMessageResponse message, CancellationToken cancellationToken = default);

        /// <summary>User-targeted: drives inbox reorder/unread badge for a specific employee, regardless of whether they have the conversation open.</summary>
        Task BroadcastConversationUpdatedAsync(long employeeId, ChatConversationSummaryResponse conversation, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted. Null reactionType means the reaction was removed/toggled off.</summary>
        Task BroadcastMessageReactedAsync(long conversationId, long chatMessageId, long employeeId, ReactionTypeEnum? reactionType, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted: everyone with the thread open sees this participant's read-receipt indicator update live.</summary>
        Task BroadcastMessageReadAsync(long conversationId, long employeeId, DateTime lastReadAt, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted: everyone with the thread open sees the updated title/photo live, without needing to reload the conversation.</summary>
        Task BroadcastGroupUpdatedAsync(long conversationId, ChatConversationResponse conversation, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted: everyone with the thread open replaces that message with the "removed" tombstone live.</summary>
        Task BroadcastMessageDeletedAsync(long conversationId, long chatMessageId, CancellationToken cancellationToken = default);
    }
}
