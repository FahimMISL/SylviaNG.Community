using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ConversationGetAllPaged
{
    public class ConversationGetAllPagedHandler : IRequestHandler<ConversationGetAllPagedQuery, PagedResult<ConversationResponse>>
    {
        private readonly IMarketplaceService _marketplaceService;

        public ConversationGetAllPagedHandler(IMarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        public async Task<PagedResult<ConversationResponse>> Handle(ConversationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _marketplaceService.GetConversationsPagedAsync(query.EmployeeId, query.Request);
        }
    }
}
