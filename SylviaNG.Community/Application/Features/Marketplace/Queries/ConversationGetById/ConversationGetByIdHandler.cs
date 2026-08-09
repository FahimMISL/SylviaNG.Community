using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ConversationGetById
{
    public class ConversationGetByIdHandler : IRequestHandler<ConversationGetByIdQuery, ConversationResponse>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ConversationGetByIdHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<ConversationResponse> Handle(ConversationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetConversationByIdAsync(query.ConversationId);
        }
    }
}
