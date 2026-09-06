using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageGetPinned
{
    public class ChatMessageGetPinnedHandler : IRequestHandler<ChatMessageGetPinnedQuery, List<ChatMessageResponse>>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageGetPinnedHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<List<ChatMessageResponse>> Handle(ChatMessageGetPinnedQuery query, CancellationToken cancellationToken)
        {
            return await _chatMessageService.GetPinnedMessagesAsync(query.ChatConversationId, query.CallerEmployeeId);
        }
    }
}
