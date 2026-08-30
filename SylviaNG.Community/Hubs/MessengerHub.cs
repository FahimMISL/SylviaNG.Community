using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Hubs
{
    /// <summary>
    /// Group-based hub for live Messenger delivery. Clients join/leave a "chat-{conversationId}"
    /// group as they open/close a thread (mirrors FeedHub's "post-{postId}" pattern); this is a
    /// second, independent entry point into conversation data, so JoinConversation re-validates
    /// participant membership itself rather than trusting the REST layer's authorization alone.
    /// User-targeted events (ConversationUpdated) go through IMessengerBroadcaster directly via
    /// Clients.User(...), which NotificationUserIdProvider already wires up hub-wide.
    /// </summary>
    [Authorize]
    public class MessengerHub : Hub<IMessengerClient>
    {
        private const string EmployeeIdClaimType = "employee_id";

        private readonly IChatConversationService _chatConversationService;

        public MessengerHub(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task JoinConversation(long conversationId)
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return;

            var isParticipant = await _chatConversationService.IsActiveParticipantAsync(conversationId, employeeId.Value);
            if (!isParticipant)
                return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{conversationId}");
        }

        public async Task LeaveConversation(long conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{conversationId}");
        }

        public async Task SendTyping(long conversationId)
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return;

            await Clients.OthersInGroup($"chat-{conversationId}").UserTyping(conversationId, employeeId.Value);
        }

        private long? GetEmployeeId()
        {
            var value = Context.User?.FindFirst(EmployeeIdClaimType)?.Value;
            return long.TryParse(value, out var employeeId) ? employeeId : null;
        }
    }
}
