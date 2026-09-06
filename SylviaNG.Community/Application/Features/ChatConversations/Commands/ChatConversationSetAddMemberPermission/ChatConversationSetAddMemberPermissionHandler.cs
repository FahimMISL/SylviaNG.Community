using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetAddMemberPermission
{
    public class ChatConversationSetAddMemberPermissionHandler : IRequestHandler<ChatConversationSetAddMemberPermissionCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationSetAddMemberPermissionHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationSetAddMemberPermissionCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.SetAddMemberPermissionAsync(command.ChatConversationId, command.OnlyAdminsCanAddMembers, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
