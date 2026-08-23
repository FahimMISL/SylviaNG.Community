using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Roles.Commands.RoleCreate;
using SylviaNG.Community.Application.Features.Roles.Commands.RoleDelete;
using SylviaNG.Community.Application.Features.Roles.Commands.RoleUpdate;
using SylviaNG.Community.Application.Features.Roles.Models;
using SylviaNG.Community.Application.Features.Roles.Queries.RoleGetAllPaged;
using SylviaNG.Community.Application.Features.Roles.Queries.RoleGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/role")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RoleResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new RoleGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<RoleResponse>> GetById(long roleId)
        {
            var result = await _mediator.Send(new RoleGetByIdQuery(roleId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RoleCreateRequest request)
        {
            var id = await _mediator.Send(new RoleCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{roleId}")]
        public async Task<ActionResult> Update(long roleId, [FromBody] RoleUpdateRequest request)
        {
            await _mediator.Send(new RoleUpdateCommand(roleId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{roleId}")]
        public async Task<ActionResult> Delete(long roleId)
        {
            await _mediator.Send(new RoleDeleteCommand(roleId));
            return Ok();
        }
    }
}
