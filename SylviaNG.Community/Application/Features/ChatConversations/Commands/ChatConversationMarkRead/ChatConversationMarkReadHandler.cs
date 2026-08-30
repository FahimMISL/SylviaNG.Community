using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationMarkRead
{
    public class ChatConversationMarkReadHandler : IRequestHandler<ChatConversationMarkReadCommand, Unit>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationMarkReadHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<Unit> Handle(ChatConversationMarkReadCommand command, CancellationToken cancellationToken)
        {
            await _chatConversationService.MarkReadAsync(command.ChatConversationId, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
