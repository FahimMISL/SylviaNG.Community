using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationCreate
{
    public class ChatConversationCreateCommand : IRequest<long>
    {
        public ChatConversationCreateRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationCreateCommand(ChatConversationCreateRequest request, long callerEmployeeId)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
