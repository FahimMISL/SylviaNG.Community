using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementUpdate
{
    public class AnnouncementUpdateHandler : IRequestHandler<AnnouncementUpdateCommand, Unit>
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementUpdateHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<Unit> Handle(AnnouncementUpdateCommand command, CancellationToken cancellationToken)
        {
            await _announcementService.UpdateAsync(command.AnnouncementId, command.Request);
            return Unit.Value;
        }
    }
}
