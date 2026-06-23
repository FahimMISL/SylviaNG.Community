using MediatR;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementDelete
{
    public class AnnouncementDeleteCommand : IRequest<Unit>
    {
        public long AnnouncementId { get; set; }

        public AnnouncementDeleteCommand(long AnnouncementId)
        {
            AnnouncementId = AnnouncementId;
        }
    }
}
