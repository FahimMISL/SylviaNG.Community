using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageSetPinned
{
    public class ChatMessageSetPinnedHandler : IRequestHandler<ChatMessageSetPinnedCommand, Unit>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageSetPinnedHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<Unit> Handle(ChatMessageSetPinnedCommand command, CancellationToken cancellationToken)
        {
            await _chatMessageService.SetPinnedAsync(command.ChatMessageId, command.CallerEmployeeId, command.IsPinned);
            return Unit.Value;
        }
    }
}
