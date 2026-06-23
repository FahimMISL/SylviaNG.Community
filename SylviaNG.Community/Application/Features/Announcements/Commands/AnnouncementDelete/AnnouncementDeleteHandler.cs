using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Announcements.Commands.AnnouncementDelete
{
    public class AnnouncementDeleteHandler : IRequestHandler<AnnouncementDeleteCommand, Unit>
    {
        private readonly IAnnouncementService _AnnouncementService;

        public AnnouncementDeleteHandler(IAnnouncementService AnnouncementService)
        {
            _AnnouncementService = AnnouncementService;
        }

        public async Task<Unit> Handle(AnnouncementDeleteCommand command, CancellationToken cancellationToken)
        {
            await _AnnouncementService.DeleteAsync(command.AnnouncementId);
            return Unit.Value;
        }
    }
}
