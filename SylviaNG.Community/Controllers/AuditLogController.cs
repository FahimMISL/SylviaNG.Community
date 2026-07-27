using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.AuditLogs.Models;
using SylviaNG.Community.Application.Features.AuditLogs.Queries.AuditLogGetAllPaged;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    /// <summary>
    /// AuditLog is an insert-only log table - rows are written inline by other code
    /// via IAuditLogService.LogAsync, so this controller only exposes a read/GET endpoint.
    /// </summary>
    [ApiController]
    [Route("community/audit-log")]
    public class AuditLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AuditLogResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new AuditLogGetAllPagedQuery(request));
            return Ok(result);
        }
    }
}
