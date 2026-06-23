using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetById
{
    public class AnnouncementGetByIdHandler : IRequestHandler<AnnouncementGetByIdQuery, AnnouncementResponse>
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementGetByIdHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<AnnouncementResponse> Handle(AnnouncementGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _announcementService.GetByIdAsync(query.AnnouncementId);
        }
    }
}
