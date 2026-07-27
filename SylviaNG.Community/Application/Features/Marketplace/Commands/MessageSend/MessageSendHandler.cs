using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MessageSend
{
    public class MessageSendHandler : IRequestHandler<MessageSendCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public MessageSendHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(MessageSendCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.SendMessageAsync(command.ConversationId, command.SenderId, command.Request);
        }
    }
}
