using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageForward
{
    public class ChatMessageForwardHandler : IRequestHandler<ChatMessageForwardCommand, Unit>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageForwardHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<Unit> Handle(ChatMessageForwardCommand command, CancellationToken cancellationToken)
        {
            await _chatMessageService.ForwardAsync(command.ChatMessageId, command.Request.ConversationIds, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
