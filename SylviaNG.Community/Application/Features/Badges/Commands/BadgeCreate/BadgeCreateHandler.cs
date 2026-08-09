using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeCreate
{
    public class BadgeCreateHandler : IRequestHandler<BadgeCreateCommand, long>
    {
        private readonly IBadgeService _badgeService;

        public BadgeCreateHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task<long> Handle(BadgeCreateCommand command, CancellationToken cancellationToken)
        {
            return await _badgeService.CreateAsync(command.Request);
        }
    }
}
