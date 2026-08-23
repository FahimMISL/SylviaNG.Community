using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskCreate;
using SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskDelete;
using SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskUpdate;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Application.Features.RecurringTasks.Queries.RecurringTaskGetAllPaged;
using SylviaNG.Community.Application.Features.RecurringTasks.Queries.RecurringTaskGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/recurring-task")]
    [Authorize(Policy = "HRAdminOnly")]
    public class RecurringTaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecurringTaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RecurringTaskResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RecurringTaskGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{recurringTaskId}")]
        public async Task<ActionResult<RecurringTaskResponse>> GetById(long recurringTaskId)
        {
            var result = await _mediator.Send(new RecurringTaskGetByIdQuery(recurringTaskId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RecurringTaskCreateRequest request)
        {
            var id = await _mediator.Send(new RecurringTaskCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{recurringTaskId}")]
        public async Task<ActionResult> Update(long recurringTaskId, [FromBody] RecurringTaskUpdateRequest request)
        {
            await _mediator.Send(new RecurringTaskUpdateCommand(recurringTaskId, request));
            return Ok();
        }

        /// <summary>US-7.12: deactivates the series (stops future generation) - already-generated
        /// Task instances are unaffected, see RecurringTaskService.DeleteAsync.</summary>
        [HttpDelete("{recurringTaskId}")]
        public async Task<ActionResult> Delete(long recurringTaskId)
        {
            await _mediator.Send(new RecurringTaskDeleteCommand(recurringTaskId));
            return Ok();
        }
    }
}
