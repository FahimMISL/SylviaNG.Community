using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate
{
    public class AnnouncementCreateHandler : IRequestHandler<AnnouncementCreateCommand, long>
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementCreateHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<long> Handle(AnnouncementCreateCommand command, CancellationToken cancellationToken)
        {
            return await _announcementService.CreateAsync(command.Request);
        }
    }
}
