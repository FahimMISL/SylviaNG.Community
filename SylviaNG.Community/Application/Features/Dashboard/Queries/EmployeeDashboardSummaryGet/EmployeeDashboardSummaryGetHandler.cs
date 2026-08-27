using MediatR;
using SylviaNG.Community.Application.Features.Dashboard.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Dashboard.Queries.EmployeeDashboardSummaryGet
{
    public class EmployeeDashboardSummaryGetHandler : IRequestHandler<EmployeeDashboardSummaryGetQuery, EmployeeDashboardSummaryResponse>
    {
        private readonly IDashboardService _dashboardService;

        public EmployeeDashboardSummaryGetHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<EmployeeDashboardSummaryResponse> Handle(EmployeeDashboardSummaryGetQuery query, CancellationToken cancellationToken)
        {
            return await _dashboardService.GetEmployeeSummaryAsync(query.EmployeeId);
        }
    }
}
