using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.MessageGetAll
{
    public class MessageGetAllHandler : IRequestHandler<MessageGetAllQuery, List<MessageResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public MessageGetAllHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<List<MessageResponse>> Handle(MessageGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetMessagesAsync(query.ConversationId);
        }
    }
}
