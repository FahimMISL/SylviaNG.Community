using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;

namespace SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetAll
{
    public class BadgeGetAllQuery : IRequest<List<BadgeResponse>>
    {
    }
}
