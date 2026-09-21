using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamCreate;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamDelete;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberAdd;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamMemberRemove;
using SylviaNG.Community.Application.Features.Teams.Commands.TeamUpdate;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Features.Teams.Queries.TeamGetAllPaged;
using SylviaNG.Community.Application.Features.Teams.Queries.TeamGetByEmployeeId;
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
            var result = await _mediator.Send(new TeamGetAllPagedQuery(request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamResponse>> GetById(long teamId)
        {
            var result = await _mediator.Send(new TeamGetByIdQuery(teamId, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TeamCreateRequest request)
        {
            // TeamService.CreateAsync only consults callerId when the caller isn't HR/Admin (the
            // "already supervises a team" check); RequireEmployeeId() is only evaluated on that
            // branch, so an Admin-type caller (no Employee record) can still create a team via HR/Admin.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            var id = await _mediator.Send(new TeamCreateCommand(request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpPut("{teamId}")]
        public async Task<ActionResult> Update(long teamId, [FromBody] TeamUpdateRequest request)
        {
            // TeamService.UpdateAsync only consults callerId when the caller isn't HR/Admin (the
            // supervisor-only check, via EnsureSupervisorOrHrAdmin); RequireEmployeeId() is only
            // evaluated on that branch, so an Admin-type caller can still edit any team.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new TeamUpdateCommand(teamId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{teamId}")]
        public async Task<ActionResult> Delete(long teamId)
        {
            // Same rationale as Update above (EnsureSupervisorOrHrAdmin bypasses callerId entirely
            // when the caller is HR/Admin).
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new TeamDeleteCommand(teamId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpGet("{teamId}/members")]
        public async Task<ActionResult<List<TeamMemberResponse>>> GetMembers(long teamId)
        {
            var result = await _mediator.Send(new TeamMemberGetAllQuery(teamId, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        /// <summary>Open read - see ITeamService.GetTeamsByEmployeeIdAsync. Used to show an
        /// employee's team affiliation elsewhere (e.g. election candidate lists), not gated to
        /// supervisor/member/HR-admin like GetById/GetMembers above.</summary>
        [HttpGet("by-employee/{employeeId}")]
        public async Task<ActionResult<List<TeamResponse>>> GetByEmployeeId(long employeeId)
        {
            var result = await _mediator.Send(new TeamGetByEmployeeIdQuery(employeeId));
            return Ok(result);
        }

        [HttpPost("{teamId}/members")]
        public async Task<ActionResult<long>> AddMember(long teamId, [FromBody] TeamMemberAddRequest request)
        {
            // Same rationale as Update above (EnsureSupervisorOrHrAdmin bypasses callerId entirely
            // when the caller is HR/Admin).
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            var id = await _mediator.Send(new TeamMemberAddCommand(teamId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpDelete("{teamId}/members/{employeeId}")]
        public async Task<ActionResult> RemoveMember(long teamId, long employeeId)
        {
            // Same rationale as Update above (EnsureSupervisorOrHrAdmin bypasses callerId entirely
            // when the caller is HR/Admin).
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new TeamMemberRemoveCommand(teamId, employeeId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }
    }
}
