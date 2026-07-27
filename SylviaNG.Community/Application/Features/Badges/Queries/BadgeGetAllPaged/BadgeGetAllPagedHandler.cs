using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetAllPaged
{
    public class BadgeGetAllPagedHandler : IRequestHandler<BadgeGetAllPagedQuery, PagedResult<BadgeResponse>>
    {
        private readonly IBadgeService _badgeService;

        public BadgeGetAllPagedHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task<PagedResult<BadgeResponse>> Handle(BadgeGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _badgeService.GetPaginatedAsync(query.Request);
        }
    }
}
