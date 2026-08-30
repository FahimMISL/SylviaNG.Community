using Microsoft.AspNetCore.SignalR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Domain.Enums;
using SylviaNG.Community.Hubs;

namespace SylviaNG.Community.Infrastructure.Services
{
    /// <summary>
    /// Lives in Infrastructure because it depends on Microsoft.AspNetCore.SignalR
    /// (same placement logic as SignalRNotificationBroadcaster/SignalRFeedBroadcaster).
    /// </summary>
    public class SignalRMessengerBroadcaster : IMessengerBroadcaster
    {
        private readonly IHubContext<MessengerHub, IMessengerClient> _hubContext;

        public SignalRMessengerBroadcaster(IHubContext<MessengerHub, IMessengerClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastMessageAsync(long conversationId, ChatMessageResponse message, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"chat-{conversationId}").ReceiveMessage(message, cancellationToken);
        }

        public async Task BroadcastConversationUpdatedAsync(long employeeId, ChatConversationSummaryResponse conversation, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.User(employeeId.ToString()).ConversationUpdated(conversation, cancellationToken);
        }

        public async Task BroadcastMessageReactedAsync(long conversationId, long chatMessageId, long employeeId, ReactionTypeEnum? reactionType, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"chat-{conversationId}").MessageReacted(chatMessageId, employeeId, reactionType, cancellationToken);
        }

        public async Task BroadcastMessageReadAsync(long conversationId, long employeeId, DateTime lastReadAt, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"chat-{conversationId}").MessageRead(conversationId, employeeId, lastReadAt, cancellationToken);
        }

        public async Task BroadcastGroupUpdatedAsync(long conversationId, ChatConversationResponse conversation, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"chat-{conversationId}").GroupUpdated(conversation, cancellationToken);
        }

        public async Task BroadcastMessageDeletedAsync(long conversationId, long chatMessageId, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients.Group($"chat-{conversationId}").MessageDeleted(conversationId, chatMessageId, cancellationToken);
        }
    }
}
