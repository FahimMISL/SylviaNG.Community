using MediatR;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetAddMemberPermission
{
    public class ChatConversationSetAddMemberPermissionCommand : IRequest<Unit>
    {
        public long ChatConversationId { get; set; }
        public bool OnlyAdminsCanAddMembers { get; set; }
        public long CallerEmployeeId { get; set; }

        public ChatConversationSetAddMemberPermissionCommand(long chatConversationId, bool onlyAdminsCanAddMembers, long callerEmployeeId)
        {
            ChatConversationId = chatConversationId;
            OnlyAdminsCanAddMembers = onlyAdminsCanAddMembers;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
