using MediatR;
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
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/team")]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public TeamController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
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

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TeamCreateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new TeamCreateCommand(request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpPut("{teamId}")]
        public async Task<ActionResult> Update(long teamId, [FromBody] TeamUpdateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new TeamUpdateCommand(teamId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{teamId}")]
        public async Task<ActionResult> Delete(long teamId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new TeamDeleteCommand(teamId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpGet("{teamId}/members")]
        public async Task<ActionResult<List<TeamMemberResponse>>> GetMembers(long teamId)
        {
            var result = await _mediator.Send(new TeamMemberGetAllQuery(teamId));
            return Ok(result);
        }

        [HttpPost("{teamId}/members")]
        public async Task<ActionResult<long>> AddMember(long teamId, [FromBody] TeamMemberAddRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new TeamMemberAddCommand(teamId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpDelete("{teamId}/members/{employeeId}")]
        public async Task<ActionResult> RemoveMember(long teamId, long employeeId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new TeamMemberRemoveCommand(teamId, employeeId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }
    }
}
