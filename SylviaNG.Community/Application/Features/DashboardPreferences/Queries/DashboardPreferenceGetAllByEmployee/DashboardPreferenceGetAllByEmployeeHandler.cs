using MediatR;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.DashboardPreferences.Queries.DashboardPreferenceGetAllByEmployee
{
    public class DashboardPreferenceGetAllByEmployeeHandler : IRequestHandler<DashboardPreferenceGetAllByEmployeeQuery, List<DashboardPreferenceResponse>>
    {
        private readonly IDashboardPreferenceService _dashboardPreferenceService;

        public DashboardPreferenceGetAllByEmployeeHandler(IDashboardPreferenceService dashboardPreferenceService)
        {
            _dashboardPreferenceService = dashboardPreferenceService;
        }

        public async Task<List<DashboardPreferenceResponse>> Handle(DashboardPreferenceGetAllByEmployeeQuery query, CancellationToken cancellationToken)
        {
            return await _dashboardPreferenceService.GetByEmployeeAsync(query.EmployeeId);
        }
    }
}
