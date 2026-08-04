using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeUpdate
{
    public class BadgeUpdateCommand : IRequest
    {
        public long BadgeId { get; set; }
        public BadgeUpdateRequest Request { get; set; }

        public BadgeUpdateCommand(long badgeId, BadgeUpdateRequest request)
        {
            BadgeId = badgeId;
            Request = request;
        }
    }
}
