using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationCreate
{
    public class ChatConversationCreateHandler : IRequestHandler<ChatConversationCreateCommand, long>
    {
        private readonly IChatConversationService _chatConversationService;

        public ChatConversationCreateHandler(IChatConversationService chatConversationService)
        {
            _chatConversationService = chatConversationService;
        }

        public async Task<long> Handle(ChatConversationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _chatConversationService.CreateAsync(command.Request, command.CallerEmployeeId);
        }
    }
}
