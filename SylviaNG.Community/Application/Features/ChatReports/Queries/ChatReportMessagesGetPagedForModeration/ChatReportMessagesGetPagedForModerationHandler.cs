using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportMessagesGetPagedForModeration
{
    public class ChatReportMessagesGetPagedForModerationHandler : IRequestHandler<ChatReportMessagesGetPagedForModerationQuery, PagedResult<ChatMessageResponse>>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatReportMessagesGetPagedForModerationHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<PagedResult<ChatMessageResponse>> Handle(ChatReportMessagesGetPagedForModerationQuery query, CancellationToken cancellationToken)
        {
            return await _chatMessageService.GetPagedForModerationAsync(query.ConversationId, query.Request);
        }
    }
}
