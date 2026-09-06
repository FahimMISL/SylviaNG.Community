using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationAddParticipants
{
    public class ChatConversationAddParticipantsHandler : IRequestHandler<ChatConversationAddParticipantsCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationAddParticipantsHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationAddParticipantsCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.AddParticipantsAsync(command.ChatConversationId, command.Request.EmployeeIds, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
