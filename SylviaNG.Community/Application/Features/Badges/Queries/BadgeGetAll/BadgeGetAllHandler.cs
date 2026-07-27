using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetAll
{
    public class BadgeGetAllHandler : IRequestHandler<BadgeGetAllQuery, List<BadgeResponse>>
    {
        private readonly IBadgeService _badgeService;

        public BadgeGetAllHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task<List<BadgeResponse>> Handle(BadgeGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _badgeService.GetAllAsync();
        }
    }
}
