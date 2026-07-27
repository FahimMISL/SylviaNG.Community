using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationPreferenceUpsert;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Notifications.Queries.NotificationPreferenceGetAllByEmployee;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/notification-preference")]
    public class NotificationPreferenceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationPreferenceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all notification category preferences for an employee.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NotificationPreferenceResponse>>> GetByEmployee([FromQuery] long employeeId)
        {
            var result = await _mediator.Send(new NotificationPreferenceGetAllByEmployeeQuery(employeeId));
            return Ok(result);
        }

        /// <summary>
        /// Upsert (create or update) the preference for an employee's notification category.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<long>> Upsert([FromBody] NotificationPreferenceUpsertRequest request)
        {
            var id = await _mediator.Send(new NotificationPreferenceUpsertCommand(request));
            return Ok(id);
        }
    }
}
