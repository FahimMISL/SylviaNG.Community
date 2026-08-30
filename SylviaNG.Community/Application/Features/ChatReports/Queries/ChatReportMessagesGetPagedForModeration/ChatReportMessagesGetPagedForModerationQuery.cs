using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportMessagesGetPagedForModeration
{
    /// <summary>HR/Admin-only read of a conversation's full message thread, bypassing the participant check - see IChatMessageService.GetPagedForModerationAsync.</summary>
    public class ChatReportMessagesGetPagedForModerationQuery : IRequest<PagedResult<ChatMessageResponse>>
    {
        public long ConversationId { get; set; }
        public PagedRequest Request { get; set; }

        public ChatReportMessagesGetPagedForModerationQuery(long conversationId, PagedRequest request)
        {
            ConversationId = conversationId;
            Request = request;
        }
    }
}
