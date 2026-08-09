using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupCreate;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupDelete;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupJoin;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestApprove;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestCreate;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupJoinRequestReject;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupLeave;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberAdd;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRemove;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupMemberRoleChange;
using SylviaNG.Community.Application.Features.Groups.Commands.GroupUpdate;
using SylviaNG.Community.Application.Features.Groups.Models;
using SylviaNG.Community.Application.Features.Groups.Queries.GroupGetAllPaged;
using SylviaNG.Community.Application.Features.Groups.Queries.GroupGetMy;
using SylviaNG.Community.Application.Features.Groups.Queries.GroupGetById;
using SylviaNG.Community.Application.Features.Groups.Queries.GroupJoinRequestGetAllPending;
using SylviaNG.Community.Application.Features.Groups.Queries.GroupMemberGetAll;
using SylviaNG.Community.Application.Features.Groups.Queries.GroupPostGetAllPaged;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/group")]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public GroupController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<GroupResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new GroupGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<GroupResponse>>> GetMy()
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new GroupGetMyQuery(callerId));
            return Ok(result);
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<GroupResponse>> GetById(long groupId)
        {
            var result = await _mediator.Send(new GroupGetByIdQuery(groupId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] GroupCreateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new GroupCreateCommand(request, callerId));
            return Ok(id);
        }

        [HttpPut("{groupId}")]
        public async Task<ActionResult> Update(long groupId, [FromBody] GroupUpdateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupUpdateCommand(groupId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{groupId}")]
        public async Task<ActionResult> Delete(long groupId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupDeleteCommand(groupId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpPost("{groupId}/join")]
        public async Task<ActionResult> Join(long groupId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupJoinCommand(groupId, callerId));
            return Ok();
        }

        [HttpPost("{groupId}/leave")]
        public async Task<ActionResult> Leave(long groupId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupLeaveCommand(groupId, callerId));
            return Ok();
        }

        [HttpPost("{groupId}/join-requests")]
        public async Task<ActionResult<long>> RequestToJoin(long groupId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new GroupJoinRequestCreateCommand(groupId, callerId));
            return Ok(id);
        }

        [HttpGet("{groupId}/join-requests")]
        public async Task<ActionResult<List<GroupJoinRequestResponse>>> GetPendingJoinRequests(long groupId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new GroupJoinRequestGetAllPendingQuery(groupId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        [HttpPut("{groupId}/join-requests/{groupJoinRequestId}/approve")]
        public async Task<ActionResult> ApproveJoinRequest(long groupId, long groupJoinRequestId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupJoinRequestApproveCommand(groupJoinRequestId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpPut("{groupId}/join-requests/{groupJoinRequestId}/reject")]
        public async Task<ActionResult> RejectJoinRequest(long groupId, long groupJoinRequestId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupJoinRequestRejectCommand(groupJoinRequestId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpGet("{groupId}/members")]
        public async Task<ActionResult<List<GroupMemberResponse>>> GetMembers(long groupId)
        {
            var result = await _mediator.Send(new GroupMemberGetAllQuery(groupId));
            return Ok(result);
        }

        [HttpPost("{groupId}/members")]
        public async Task<ActionResult<long>> AddMember(long groupId, [FromBody] GroupMemberAddRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new GroupMemberAddCommand(groupId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpDelete("{groupId}/members/{employeeId}")]
        public async Task<ActionResult> RemoveMember(long groupId, long employeeId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupMemberRemoveCommand(groupId, employeeId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpPut("{groupId}/members/role")]
        public async Task<ActionResult> ChangeMemberRole(long groupId, [FromBody] GroupMemberRoleChangeRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new GroupMemberRoleChangeCommand(groupId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpGet("{groupId}/posts")]
        public async Task<ActionResult<PagedResult<PostResponse>>> GetPosts(long groupId, [FromQuery] PostFilterRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new GroupPostGetAllPagedQuery(groupId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }
    }
}
