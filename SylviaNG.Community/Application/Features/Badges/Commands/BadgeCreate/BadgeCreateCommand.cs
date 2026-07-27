using MediatR;
using SylviaNG.Community.Application.Features.Badges.Models;

namespace SylviaNG.Community.Application.Features.Badges.Commands.BadgeCreate
{
    public class BadgeCreateCommand : IRequest<long>
    {
        public BadgeCreateRequest Request { get; set; }

        public BadgeCreateCommand(BadgeCreateRequest request)
        {
            Request = request;
        }
    }
}
