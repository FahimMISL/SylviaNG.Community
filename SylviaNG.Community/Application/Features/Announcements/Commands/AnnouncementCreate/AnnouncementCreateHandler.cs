using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementCreate
{
    public class AnnouncementCreateHandler : IRequestHandler<AnnouncementCreateCommand, long>
    {
        private readonly IAnnouncementService _AnnouncementService;

        public AnnouncementCreateHandler(IAnnouncementService AnnouncementService)
        {
            _AnnouncementService = AnnouncementService;
        }

        public async Task<long> Handle(AnnouncementCreateCommand command, CancellationToken cancellationToken)
        {
            return await _AnnouncementService.CreateAsync(command.Request);
        }
    }
}
