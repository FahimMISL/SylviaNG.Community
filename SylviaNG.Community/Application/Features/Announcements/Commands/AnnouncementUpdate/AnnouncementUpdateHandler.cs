using MediatR;
using SylviaNG.Community.Application.Features.Announcements.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementUpdate
{
    public class AnnouncementUpdateHandler : IRequestHandler<AnnouncementUpdateCommand, Unit>
    {
        private readonly IAnnouncementService _AnnouncementService;

        public AnnouncementUpdateHandler(IAnnouncementService AnnouncementService)
        {
            _AnnouncementService = AnnouncementService;
        }

        public async Task<Unit> Handle(AnnouncementUpdateCommand command, CancellationToken cancellationToken)
        {
            await _AnnouncementService.UpdateAsync(command.AnnouncementId, command.Request);
            return Unit.Value;
        }
    }
}
