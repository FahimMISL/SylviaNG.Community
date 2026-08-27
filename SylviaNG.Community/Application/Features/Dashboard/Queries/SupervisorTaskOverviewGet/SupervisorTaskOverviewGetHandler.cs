using MediatR;
using SylviaNG.Community.Application.Features.Dashboard.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Dashboard.Queries.SupervisorTaskOverviewGet
{
    public class SupervisorTaskOverviewGetHandler : IRequestHandler<SupervisorTaskOverviewGetQuery, SupervisorTaskOverviewResponse>
    {
        private readonly IDashboardService _dashboardService;

        public SupervisorTaskOverviewGetHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<SupervisorTaskOverviewResponse> Handle(SupervisorTaskOverviewGetQuery query, CancellationToken cancellationToken)
        {
            return await _dashboardService.GetSupervisorTaskOverviewAsync(query.SupervisorEmployeeId);
        }
    }
}
