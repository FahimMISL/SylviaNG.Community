using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementDelete
{
    public class AnnouncementDeleteHandler : IRequestHandler<AnnouncementDeleteCommand, Unit>
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementDeleteHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<Unit> Handle(AnnouncementDeleteCommand command, CancellationToken cancellationToken)
        {
            await _announcementService.DeleteAsync(command.AnnouncementId);
            return Unit.Value;
        }
    }
}
