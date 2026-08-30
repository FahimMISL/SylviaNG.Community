using MediatR;
using SylviaNG.Community.Application.Features.ChatConversations.Models;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationUpdateGroup
{
    public class ChatConversationUpdateGroupCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public ChatConversationUpdateGroupRequest Request { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationUpdateGroupCommand(long chatConversationId, ChatConversationUpdateGroupRequest request, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
