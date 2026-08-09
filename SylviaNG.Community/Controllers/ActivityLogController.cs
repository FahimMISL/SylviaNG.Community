using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.ActivityLogs.Models;
using SylviaNG.Community.Application.Features.ActivityLogs.Queries.ActivityLogGetAllPaged;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    /// <summary>
    /// ActivityLog is an insert-only log table - rows are written inline by other code
    /// via IActivityLogService.LogAsync, so this controller only exposes a read/GET endpoint.
    /// </summary>
    [ApiController]
    [Route("community/activity-log")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActivityLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ActivityLogResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ActivityLogGetAllPagedQuery(request));
            return Ok(result);
        }
    }
}
