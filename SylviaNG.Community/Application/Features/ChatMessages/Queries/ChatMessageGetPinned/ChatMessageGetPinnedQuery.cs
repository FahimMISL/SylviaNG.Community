using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;

namespace SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageGetPinned
{
    public class ChatMessageGetPinnedQuery : IRequest<List<ChatMessageResponse>>
    {
        public long ChatConversationId { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatMessageGetPinnedQuery(long chatConversationId, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
