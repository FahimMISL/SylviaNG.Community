using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ConversationStart
{
    public class ConversationStartHandler : IRequestHandler<ConversationStartCommand, long>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ConversationStartHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<long> Handle(ConversationStartCommand command, CancellationToken cancellationToken)
        {
            return await _marketplaceService.StartConversationAsync(command.InitiatorEmployeeId, command.Request);
        }
    }
}
