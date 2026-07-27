using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceDelete
{
    public class DashboardPreferenceDeleteHandler : IRequestHandler<DashboardPreferenceDeleteCommand>
    {
        private readonly IDashboardPreferenceService _dashboardPreferenceService;

        public DashboardPreferenceDeleteHandler(IDashboardPreferenceService dashboardPreferenceService)
        {
            _dashboardPreferenceService = dashboardPreferenceService;
        }

        public async Task Handle(DashboardPreferenceDeleteCommand command, CancellationToken cancellationToken)
        {
            await _dashboardPreferenceService.DeleteAsync(command.PreferenceId);
        }
    }
}
