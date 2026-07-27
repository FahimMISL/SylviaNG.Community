using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamDelete;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberAdd;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberRemove;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamUpdate;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Features.Teams.Queries.TeamGetAllPaged;
using SylviaNG.Community.Application.Features.Teams.Queries.TeamGetById;
using SylviaNG.Community.Application.Features.Teams.Queries.TeamMemberGetAll;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/team")]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<TeamResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new TeamGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamResponse>> GetById(long teamId)
        {
            var result = await _mediator.Send(new TeamGetByIdQuery(teamId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TeamCreateRequest request)
        {
            var id = await _mediator.Send(new TeamCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{teamId}")]
        public async Task<ActionResult> Update(long teamId, [FromBody] TeamUpdateRequest request)
        {
            await _mediator.Send(new TeamUpdateCommand(teamId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{teamId}")]
        public async Task<ActionResult> Delete(long teamId)
        {
            await _mediator.Send(new TeamDeleteCommand(teamId));
            return Ok();
        }

        [HttpGet("{teamId}/members")]
        public async Task<ActionResult<List<TeamMemberResponse>>> GetMembers(long teamId)
        {
            var result = await _mediator.Send(new TeamMemberGetAllQuery(teamId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost("{teamId}/members")]
        public async Task<ActionResult<long>> AddMember(long teamId, [FromBody] TeamMemberAddRequest request)
        {
            var id = await _mediator.Send(new TeamMemberAddCommand(teamId, request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{teamId}/members/{employeeId}")]
        public async Task<ActionResult> RemoveMember(long teamId, long employeeId)
        {
            await _mediator.Send(new TeamMemberRemoveCommand(teamId, employeeId));
            return Ok();
        }
    }
}
