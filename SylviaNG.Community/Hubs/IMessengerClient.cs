using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Strongly-typed client contract for <see cref="MessengerHub"/>.
    /// Defines the methods the server can invoke on connected clients.
    /// </summary>
    public interface IMessengerClient
    {
        // The trailing CancellationToken is SignalR's typed-client idiom: it is used only to
        // cancel the local send operation and is stripped before the call is sent over the wire.

        /// <summary>Group-targeted: pushed to everyone with this conversation open.</summary>
        Task ReceiveMessage(ChatMessageResponse message, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted, ephemeral - no DB write behind this.</summary>
        Task UserTyping(long conversationId, long employeeId, CancellationToken cancellationToken = default);

        /// <summary>User-targeted: drives inbox reorder/unread badge regardless of which conversation (if any) the recipient has open.</summary>
        Task ConversationUpdated(ChatConversationSummaryResponse conversation, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted. Null reactionType means the reaction was removed/toggled off.</summary>
        Task MessageReacted(long chatMessageId, long employeeId, ReactionTypeEnum? reactionType, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted: lets everyone with the thread open update that participant's read-receipt indicator live.</summary>
        Task MessageRead(long conversationId, long employeeId, DateTime lastReadAt, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted: pushed to everyone with this conversation open when its title/photo changes.</summary>
        Task GroupUpdated(ChatConversationResponse conversation, CancellationToken cancellationToken = default);

        /// <summary>Group-targeted: pushed when a message is removed, so open threads can swap it for the tombstone live.</summary>
        Task MessageDeleted(long conversationId, long chatMessageId, CancellationToken cancellationToken = default);
    }
}
