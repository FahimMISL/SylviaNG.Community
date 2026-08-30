using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReact
{
    public class ChatMessageReactHandler : IRequestHandler<ChatMessageReactCommand, ChatMessageReactionResponse?>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageReactHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<ChatMessageReactionResponse?> Handle(ChatMessageReactCommand command, CancellationToken cancellationToken)
        {
            return await _chatMessageService.ReactAsync(command.ChatMessageId, command.ReactionType, command.CallerEmployeeId);
        }
    }
}
