using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatConversations.Queries.ChatConversationGetAllPaged
{
    public class ChatConversationGetAllPagedHandler : IRequestHandler<ChatConversationGetAllPagedQuery, PagedResult<ChatConversationSummaryResponse>>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationGetAllPagedHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<PagedResult<ChatConversationSummaryResponse>> Handle(ChatConversationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _chatConversationService.GetMyConversationsPagedAsync(query.CallerEmployeeId, query.Request);
        }
    }
}
