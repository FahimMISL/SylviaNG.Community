using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate
{
    public class AnnouncementCreateCommand : IRequest<long>
    {
        public AnnouncementCreateRequest Request { get; set; }

        public AnnouncementCreateCommand(AnnouncementCreateRequest request)
        {
            Request = request;
        }
    }
}
