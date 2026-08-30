using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageDelete
{
    public class ChatMessageDeleteHandler : IRequestHandler<ChatMessageDeleteCommand, Unit>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageDeleteHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<Unit> Handle(ChatMessageDeleteCommand command, CancellationToken cancellationToken)
        {
            await _chatMessageService.DeleteAsync(command.ChatMessageId, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
