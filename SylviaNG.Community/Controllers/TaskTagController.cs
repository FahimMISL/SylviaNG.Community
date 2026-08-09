using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagCreate;
using SylviaNG.Community.Application.Features.TaskTags.Commands.TaskTagDelete;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Features.TaskTags.Queries.TaskTagGetAllPaged;
using SylviaNG.Community.Application.Features.TaskTags.Queries.TaskTagGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/task-tag")]
    public class TaskTagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskTagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<TaskTagResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new TaskTagGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{tagId}")]
        public async Task<ActionResult<TaskTagResponse>> GetById(long tagId)
        {
            var result = await _mediator.Send(new TaskTagGetByIdQuery(tagId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TaskTagCreateRequest request)
        {
            var id = await _mediator.Send(new TaskTagCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{tagId}")]
        public async Task<ActionResult> Delete(long tagId)
        {
            await _mediator.Send(new TaskTagDeleteCommand(tagId));
            return Ok();
        }
    }
}
