using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Dashboard.Models;
using SylviaNG.Community.Application.Features.Dashboard.Queries.AdminDashboardSummaryGet;
using SylviaNG.Community.Application.Features.Dashboard.Queries.EmployeeDashboardSummaryGet;
using SylviaNG.Community.Application.Features.Dashboard.Queries.SupervisorTaskOverviewGet;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    /// <summary>
    /// Feature 8: Dashboard Widgets (US-8.1-8.3) - role-aware landing-page summaries, aggregated
    /// server-side. Distinct from DashboardPreferenceController, which only stores per-widget
    /// layout/visibility settings, not widget data.
    /// </summary>
    [ApiController]
    [Route("community/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public DashboardController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>US-8.1: my personal dashboard widgets. EmployeeId is always the caller's own.</summary>
        [HttpGet("employee-summary")]
        public async Task<ActionResult<EmployeeDashboardSummaryResponse>> GetEmployeeSummary()
        {
            var employeeId = _currentUserService.EmployeeId
                ?? throw new UnauthorizedException("Only an authenticated employee has a personal dashboard.");

            var result = await _mediator.Send(new EmployeeDashboardSummaryGetQuery(employeeId));
            return Ok(result);
        }

        /// <summary>
        /// US-8.2: the caller's "everything I've assigned" task overview. Any authenticated
        /// employee may call this - it simply returns zeroed stats if they supervise no teams,
        /// so the frontend should still gate the card itself on employee-summary's IsSupervisor.
        /// </summary>
        [HttpGet("supervisor-task-overview")]
        public async Task<ActionResult<SupervisorTaskOverviewResponse>> GetSupervisorTaskOverview()
        {
            var employeeId = _currentUserService.EmployeeId
                ?? throw new UnauthorizedException("Only an authenticated employee can have assigned tasks.");

            var result = await _mediator.Send(new SupervisorTaskOverviewGetQuery(employeeId));
            return Ok(result);
        }

        /// <summary>US-8.3: company-wide operational summary.</summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("admin-summary")]
        public async Task<ActionResult<AdminDashboardSummaryResponse>> GetAdminSummary()
        {
            var result = await _mediator.Send(new AdminDashboardSummaryGetQuery());
            return Ok(result);
        }
    }
}
