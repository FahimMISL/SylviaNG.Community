using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportConversationGetForModeration
{
    public class ChatReportConversationGetForModerationHandler : IRequestHandler<ChatReportConversationGetForModerationQuery, ChatConversationResponse>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatReportConversationGetForModerationHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<ChatConversationResponse> Handle(ChatReportConversationGetForModerationQuery query, CancellationToken cancellationToken)
        {
            return await _chatConversationService.GetForModerationAsync(query.ConversationId);
        }
    }
}
