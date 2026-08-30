using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageGetAllPaged
{
    public class ChatMessageGetAllPagedHandler : IRequestHandler<ChatMessageGetAllPagedQuery, PagedResult<ChatMessageResponse>>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageGetAllPagedHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<PagedResult<ChatMessageResponse>> Handle(ChatMessageGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _chatMessageService.GetPagedAsync(query.ChatConversationId, query.CallerEmployeeId, query.Request);
        }
    }
}
