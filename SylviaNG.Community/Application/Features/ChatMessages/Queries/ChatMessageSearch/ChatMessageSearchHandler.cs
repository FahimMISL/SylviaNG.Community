using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageSearch
{
    public class ChatMessageSearchHandler : IRequestHandler<ChatMessageSearchQuery, PagedResult<ChatMessageResponse>>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageSearchHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<PagedResult<ChatMessageResponse>> Handle(ChatMessageSearchQuery query, CancellationToken cancellationToken)
        {
            return await _chatMessageService.SearchAsync(query.CallerEmployeeId, query.SearchTerm, query.Request);
        }
    }
}
