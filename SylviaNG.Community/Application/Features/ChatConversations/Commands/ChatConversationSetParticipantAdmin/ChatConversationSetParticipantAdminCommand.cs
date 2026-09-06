using MediatR;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetParticipantAdmin
{
    public class ChatConversationSetParticipantAdminCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public long TargetEmployeeId { get; set; }
        public bool IsAdmin { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationSetParticipantAdminCommand(long chatConversationId, long targetEmployeeId, bool isAdmin, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            TargetEmployeeId = targetEmployeeId;
            IsAdmin = isAdmin;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
