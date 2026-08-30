using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetMuted
{
    public class ChatConversationSetMutedHandler : IRequestHandler<ChatConversationSetMutedCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationSetMutedHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationSetMutedCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.SetMutedAsync(command.ChatConversationId, command.CallerEmployeeId, command.IsMuted);
            return Unit.Value;
        }
    }
}
