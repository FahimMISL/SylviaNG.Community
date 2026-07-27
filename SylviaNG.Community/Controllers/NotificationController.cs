using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationCreate;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationDelete;
using SylviaNG.Community.Application.Features.Notifications.Commands.NotificationMarkRead;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetAllPaged;
using SylviaNG.Community.Application.Features.Notifications.Queries.NotificationGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get paginated notifications for a given employee (typically the current employee).
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<NotificationResponse>>> GetPaged([FromQuery] long employeeId, [FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new NotificationGetAllPagedQuery(employeeId, request));
            return Ok(result);
        }

        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationResponse>> GetById(long notificationId)
        {
            var result = await _mediator.Send(new NotificationGetByIdQuery(notificationId));
            return Ok(result);
        }

        /// <summary>
        /// Create a notification for an employee - called by other services/features
        /// whenever something needs to notify an employee.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] NotificationCreateRequest request)
        {
            var id = await _mediator.Send(new NotificationCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{notificationId}/read")]
        public async Task<ActionResult> MarkAsRead(long notificationId)
        {
            await _mediator.Send(new NotificationMarkReadCommand(notificationId));
            return Ok();
        }

        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> Delete(long notificationId)
        {
            await _mediator.Send(new NotificationDeleteCommand(notificationId));
            return Ok();
        }
    }
}
