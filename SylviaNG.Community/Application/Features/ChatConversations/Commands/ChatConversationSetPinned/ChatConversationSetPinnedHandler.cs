using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetPinned
{
    public class ChatConversationSetPinnedHandler : IRequestHandler<ChatConversationSetPinnedCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationSetPinnedHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationSetPinnedCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.SetPinnedAsync(command.ChatConversationId, command.CallerEmployeeId, command.IsPinned);
            return Unit.Value;
        }
    }
}
