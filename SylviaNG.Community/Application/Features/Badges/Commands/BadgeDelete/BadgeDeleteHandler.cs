using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeDelete
{
    public class BadgeDeleteHandler : IRequestHandler<BadgeDeleteCommand>
    {
        private readonly IBadgeService _badgeService;

        public BadgeDeleteHandler(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        public async Task Handle(BadgeDeleteCommand command, CancellationToken cancellationToken)
        {
            await _badgeService.DeleteAsync(command.BadgeId);
        }
    }
}
