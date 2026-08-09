using MediatR;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeDelete
{
    public class BadgeDeleteCommand : IRequest
    {
        public long BadgeId { get; set; }

        public BadgeDeleteCommand(long badgeId)
        {
            BadgeId = badgeId;
        }
    }
}
