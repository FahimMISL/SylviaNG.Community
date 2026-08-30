using MediatR;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageSend
{
    public class ChatMessageSendHandler : IRequestHandler<ChatMessageSendCommand, ChatMessageResponse>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageSendHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<ChatMessageResponse> Handle(ChatMessageSendCommand command, CancellationToken cancellationToken)
        {
            return await _chatMessageService.SendAsync(command.ChatConversationId, command.Request, command.CallerEmployeeId);
        }
    }
}
