using SylviaNG.Community.Application.Features.Dashboard.Models;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    /// <summary>Feature 8 (US-8.1-8.3): role-aware dashboard summaries, aggregated server-side.</summary>
    public interface IDashboardService
    {
        Task<EmployeeDashboardSummaryResponse> GetEmployeeSummaryAsync(long employeeId);
        Task<SupervisorTaskOverviewResponse> GetSupervisorTaskOverviewAsync(long supervisorEmployeeId);
        Task<AdminDashboardSummaryResponse> GetAdminSummaryAsync();
    }
}
