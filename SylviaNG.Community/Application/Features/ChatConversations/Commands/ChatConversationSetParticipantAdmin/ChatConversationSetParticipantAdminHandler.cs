using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetParticipantAdmin
{
    public class ChatConversationSetParticipantAdminHandler : IRequestHandler<ChatConversationSetParticipantAdminCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationSetParticipantAdminHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationSetParticipantAdminCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.SetParticipantAdminAsync(
                command.ChatConversationId, command.TargetEmployeeId, command.IsAdmin, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
