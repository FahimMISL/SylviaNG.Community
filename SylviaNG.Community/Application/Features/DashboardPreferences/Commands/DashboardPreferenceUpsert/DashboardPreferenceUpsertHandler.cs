using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceUpsert
{
    public class DashboardPreferenceUpsertHandler : IRequestHandler<DashboardPreferenceUpsertCommand, long>
    {
        private readonly IDashboardPreferenceService _dashboardPreferenceService;

        public DashboardPreferenceUpsertHandler(IDashboardPreferenceService dashboardPreferenceService)
        {
            _dashboardPreferenceService = dashboardPreferenceService;
        }

        public async Task<long> Handle(DashboardPreferenceUpsertCommand command, CancellationToken cancellationToken)
        {
            return await _dashboardPreferenceService.UpsertAsync(command.Request);
        }
    }
}
