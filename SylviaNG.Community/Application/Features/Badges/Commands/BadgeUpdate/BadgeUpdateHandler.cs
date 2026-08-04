using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeUpdate
{
    public class BadgeUpdateHandler : IRequestHandler<BadgeUpdateCommand>
    {
        private readonly IBadgeService _badgeService;

        public BadgeUpdateHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task Handle(BadgeUpdateCommand command, CancellationToken cancellationToken)
        {
            await _badgeService.UpdateAsync(command.BadgeId, command.Request);
        }
    }
}
