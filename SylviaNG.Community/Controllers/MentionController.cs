using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Mentions.Commands.MentionCreate;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetAllPaged;
using SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetByEntity;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/mention")]
    public class MentionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public MentionController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Mentions of the current caller ("mentions of me").
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<MentionResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new MentionGetAllPagedQuery(employeeId, request));
            return Ok(result);
        }

        /// <summary>
        /// Records a mention - normally created internally by the post/comment create
        /// flow when @mentions are detected in Content; exposed here for explicit use.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] MentionCreateRequest request)
        {
            var id = await _mediator.Send(new MentionCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// All mentions recorded against a specific Post or PostComment - lets the client
        /// resolve which "@Name" occurrences in that content link to which employeeId.
        /// </summary>
        [HttpGet("by-entity")]
        public async Task<ActionResult<List<MentionResponse>>> GetByEntity([FromQuery] string entityType, [FromQuery] long entityId)
        {
            var result = await _mediator.Send(new MentionGetByEntityQuery(entityType, entityId));
            return Ok(result);
        }
    }
}
