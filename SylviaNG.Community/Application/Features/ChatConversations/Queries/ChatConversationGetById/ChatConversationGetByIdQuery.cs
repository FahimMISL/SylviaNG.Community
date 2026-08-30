using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;

namespace SylviaNG.Community.Application.Features.ChatConversations.Queries.ChatConversationGetById
{
    public class ChatConversationGetByIdQuery : IRequest<ChatConversationResponse>
    {
        public long ChatConversationId { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationGetByIdQuery(long chatConversationId, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
