using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;

namespace SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportConversationGetForModeration
{
    /// <summary>HR/Admin-only read of a conversation's metadata, bypassing the participant check - see IChatConversationService.GetForModerationAsync.</summary>
    public class ChatReportConversationGetForModerationQuery : IRequest<ChatConversationResponse>
    {
        public long ConversationId { get; set; }

        public ChatReportConversationGetForModerationQuery(long conversationId)
        {
            ConversationId = conversationId;
        }
    }
}
