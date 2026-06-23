using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetById
{
    public class AnnouncementGetByIdQuery : IRequest<AnnouncementResponse>
    {
        public long AnnouncementId { get; set; }

        public AnnouncementGetByIdQuery(long announcementId)
        {
            AnnouncementId = announcementId;
        }
    }
}
