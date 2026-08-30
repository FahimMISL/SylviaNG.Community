using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Queries.ChatConversationGetById
{
    public class ChatConversationGetByIdHandler : IRequestHandler<ChatConversationGetByIdQuery, ChatConversationResponse>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationGetByIdHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<ChatConversationResponse> Handle(ChatConversationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _chatConversationService.GetByIdAsync(query.ChatConversationId, query.CallerEmployeeId);
        }
    }
}
