using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;

namespace SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetById
{
    public class BadgeGetByIdQuery : IRequest<BadgeResponse>
    {
        public long BadgeId { get; set; }

        public BadgeGetByIdQuery(long badgeId)
        {
            BadgeId = badgeId;
        }
    }
}
