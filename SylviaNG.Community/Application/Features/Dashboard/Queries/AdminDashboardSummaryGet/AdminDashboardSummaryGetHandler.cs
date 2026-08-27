using MediatR;
using SylviaNG.Community.Application.Features.Dashboard.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Dashboard.Queries.AdminDashboardSummaryGet
{
    public class AdminDashboardSummaryGetHandler : IRequestHandler<AdminDashboardSummaryGetQuery, AdminDashboardSummaryResponse>
    {
        private readonly IDashboardService _dashboardService;

        public AdminDashboardSummaryGetHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<AdminDashboardSummaryResponse> Handle(AdminDashboardSummaryGetQuery query, CancellationToken cancellationToken)
        {
            return await _dashboardService.GetAdminSummaryAsync();
        }
    }
}
