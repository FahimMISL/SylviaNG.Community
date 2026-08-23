using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Designations.Commands.DesignationCreate;
using SylviaNG.Community.Application.Features.Designations.Commands.DesignationDelete;
using SylviaNG.Community.Application.Features.Designations.Commands.DesignationUpdate;
using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetAllPaged;
using SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/designation")]
    public class DesignationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DesignationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<DesignationResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new DesignationGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{designationId}")]
        public async Task<ActionResult<DesignationResponse>> GetById(long designationId)
        {
            var result = await _mediator.Send(new DesignationGetByIdQuery(designationId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DesignationCreateRequest request)
        {
            var id = await _mediator.Send(new DesignationCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{designationId}")]
        public async Task<ActionResult> Update(long designationId, [FromBody] DesignationUpdateRequest request)
        {
            await _mediator.Send(new DesignationUpdateCommand(designationId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{designationId}")]
        public async Task<ActionResult> Delete(long designationId)
        {
            await _mediator.Send(new DesignationDeleteCommand(designationId));
            return Ok();
        }
    }
}
