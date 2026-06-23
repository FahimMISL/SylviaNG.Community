using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAll
{
    public class AnnouncementGetAllHandler : IRequestHandler<AnnouncementGetAllQuery, List<AnnouncementResponse>>
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementGetAllHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<List<AnnouncementResponse>> Handle(AnnouncementGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _announcementService.GetAllAsync();
        }
    }
}
