using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationAddParticipants
{
    public class ChatConversationAddParticipantsCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public ChatConversationAddParticipantsRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationAddParticipantsCommand(long chatConversationId, ChatConversationAddParticipantsRequest request, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
