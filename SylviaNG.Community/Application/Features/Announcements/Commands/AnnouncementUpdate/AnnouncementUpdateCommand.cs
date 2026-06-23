using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementUpdate
{
    public class AnnouncementUpdateCommand : IRequest<Unit>
    {
        public long AnnouncementId { get; set; }
        public AnnouncementUpdateRequest Request { get; set; }

        public AnnouncementUpdateCommand(long AnnouncementId, AnnouncementUpdateRequest request)
        {
            AnnouncementId = AnnouncementId;
            Request = request;
        }
    }
}
