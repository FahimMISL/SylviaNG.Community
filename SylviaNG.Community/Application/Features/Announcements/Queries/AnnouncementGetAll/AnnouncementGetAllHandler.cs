using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Queries.AnnouncementGetAll
{
    public class AnnouncementGetAllHandler : IRequestHandler<AnnouncementGetAllQuery, List<AnnouncementResponse>>
    {
        private readonly IAnnouncementService _AnnouncementService;

        public AnnouncementGetAllHandler(IAnnouncementService AnnouncementService)
        {
            _AnnouncementService = AnnouncementService;
        }

        public async Task<List<AnnouncementResponse>> Handle(AnnouncementGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _AnnouncementService.GetAllAsync();
        }
    }
}
