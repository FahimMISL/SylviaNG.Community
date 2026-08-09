using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetById
{
    public class BadgeGetByIdHandler : IRequestHandler<BadgeGetByIdQuery, BadgeResponse>
    {
        private readonly IBadgeService _badgeService;

        public BadgeGetByIdHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task<BadgeResponse> Handle(BadgeGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _badgeService.GetByIdAsync(query.BadgeId);
        }
    }
}
