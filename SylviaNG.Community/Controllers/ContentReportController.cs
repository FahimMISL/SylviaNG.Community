using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportCreate;
using SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportResolve;
using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Application.Features.ContentReports.Queries.ContentReportGetAllPaged;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/content-report")]
    public class ContentReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContentReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ContentReportCreateRequest request)
        {
            var id = await _mediator.Send(new ContentReportCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Moderation queue.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ContentReportResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ContentReportGetAllPagedQuery(request));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{reportId}/resolve")]
        public async Task<ActionResult> Resolve(long reportId, [FromBody] ContentReportResolveRequest request)
        {
            await _mediator.Send(new ContentReportResolveCommand(reportId, request));
            return Ok();
        }
    }
}
