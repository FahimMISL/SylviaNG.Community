using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationUpdateGroup
{
    public class ChatConversationUpdateGroupHandler : IRequestHandler<ChatConversationUpdateGroupCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationUpdateGroupHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationUpdateGroupCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.UpdateGroupAsync(command.ChatConversationId, command.Request, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
