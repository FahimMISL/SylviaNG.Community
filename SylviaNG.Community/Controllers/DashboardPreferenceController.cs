using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceDelete;
using SylviaNG.Community.Application.Features.DashboardPreferences.Commands.DashboardPreferenceUpsert;
using SylviaNG.Community.Application.Features.DashboardPreferences.Models;
using SylviaNG.Community.Application.Features.DashboardPreferences.Queries.DashboardPreferenceGetAllByEmployee;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/dashboard-preference")]
    public class DashboardPreferenceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardPreferenceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all "my dashboard" widget preferences for an employee.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<DashboardPreferenceResponse>>> GetByEmployee([FromQuery] long employeeId)
        {
            var result = await _mediator.Send(new DashboardPreferenceGetAllByEmployeeQuery(employeeId));
            return Ok(result);
        }

        /// <summary>
        /// Upsert (create or update) an employee's preference for a dashboard widget.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<long>> Upsert([FromBody] DashboardPreferenceUpsertRequest request)
        {
            var id = await _mediator.Send(new DashboardPreferenceUpsertCommand(request));
            return Ok(id);
        }

        [HttpDelete("{preferenceId}")]
        public async Task<ActionResult> Delete(long preferenceId)
        {
            await _mediator.Send(new DashboardPreferenceDeleteCommand(preferenceId));
            return Ok();
        }
    }
}
