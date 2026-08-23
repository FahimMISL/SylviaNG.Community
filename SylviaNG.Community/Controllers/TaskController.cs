using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentAdd;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentRemove;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkCancel;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskBulkReassign;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskCommentAdd;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskDelete;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskAttachmentGetAll;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskCommentGetAll;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAllPaged;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAssignedByMePaged;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetById;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetMyPaged;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetTeamPaged;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskHistoryGetAll;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskReportGenerate;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/task")]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public TaskController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>Unscoped - HR/Admin only. Everyone else uses the scoped endpoints below
        /// (my/assigned-by-me/team/{teamId}), which never trust a client-supplied AssignedTo/AssignedBy.</summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<TaskResponse>>> GetPaged([FromQuery] TaskFilterRequest request)
        {
            var result = await _mediator.Send(new TaskGetAllPagedQuery(request));
            return Ok(result);
        }

        /// <summary>US-7.8: tasks assigned to me.</summary>
        [HttpGet("my")]
        public async Task<ActionResult<PagedResult<TaskResponse>>> GetMy([FromQuery] TaskFilterRequest request)
        {
            var result = await _mediator.Send(new TaskGetMyPagedQuery(request, _currentUserService.EmployeeId));
            return Ok(result);
        }

        /// <summary>US-7.7: individual (non-team) tasks I've assigned to others.</summary>
        [HttpGet("assigned-by-me")]
        public async Task<ActionResult<PagedResult<TaskResponse>>> GetAssignedByMe([FromQuery] TaskFilterRequest request)
        {
            var result = await _mediator.Send(new TaskGetAssignedByMePagedQuery(request, _currentUserService.EmployeeId));
            return Ok(result);
        }

        /// <summary>US-7.5: a team's task board. Caller must be that team's Supervisor, or HR/Admin.</summary>
        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<PagedResult<TaskResponse>>> GetForTeam(long teamId, [FromQuery] TaskFilterRequest request)
        {
            var result = await _mediator.Send(new TaskGetTeamPagedQuery(teamId, request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskResponse>> GetById(long taskId)
        {
            var result = await _mediator.Send(new TaskGetByIdQuery(taskId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] TaskCreateRequest request)
        {
            var id = await _mediator.Send(new TaskCreateCommand(request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpPut("{taskId}")]
        public async Task<ActionResult> Update(long taskId, [FromBody] TaskUpdateRequest request)
        {
            await _mediator.Send(new TaskUpdateCommand(taskId, request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{taskId}")]
        public async Task<ActionResult> Delete(long taskId)
        {
            await _mediator.Send(new TaskDeleteCommand(taskId, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        /// <summary>US-7.13: reassign multiple tasks to a new assignee in one transaction.</summary>
        [HttpPost("bulk-reassign")]
        public async Task<ActionResult> BulkReassign([FromBody] TaskBulkReassignRequest request)
        {
            await _mediator.Send(new TaskBulkReassignCommand(request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        /// <summary>US-7.13: permanently cancel multiple tasks in one transaction.</summary>
        [HttpPost("bulk-cancel")]
        public async Task<ActionResult> BulkCancel([FromBody] TaskBulkCancelRequest request)
        {
            await _mediator.Send(new TaskBulkCancelCommand(request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpGet("{taskId}/comments")]
        public async Task<ActionResult<List<TaskCommentResponse>>> GetComments(long taskId)
        {
            var result = await _mediator.Send(new TaskCommentGetAllQuery(taskId));
            return Ok(result);
        }

        [HttpPost("{taskId}/comments")]
        public async Task<ActionResult<long>> AddComment(long taskId, [FromBody] TaskCommentAddRequest request)
        {
            var id = await _mediator.Send(new TaskCommentAddCommand(taskId, request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpGet("{taskId}/attachments")]
        public async Task<ActionResult<List<TaskAttachmentResponse>>> GetAttachments(long taskId)
        {
            var result = await _mediator.Send(new TaskAttachmentGetAllQuery(taskId));
            return Ok(result);
        }

        [HttpPost("{taskId}/attachments")]
        public async Task<ActionResult<long>> AddAttachment(long taskId, [FromBody] TaskAttachmentAddRequest request)
        {
            var id = await _mediator.Send(new TaskAttachmentAddCommand(taskId, request, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(id);
        }

        [HttpDelete("{taskId}/attachments/{attachmentId}")]
        public async Task<ActionResult> RemoveAttachment(long taskId, long attachmentId)
        {
            await _mediator.Send(new TaskAttachmentRemoveCommand(taskId, attachmentId, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        /// <summary>
        /// TaskHistory is an insert-only log - list only, no create/update/delete surface here.
        /// </summary>
        [HttpGet("{taskId}/history")]
        public async Task<ActionResult<List<TaskHistoryResponse>>> GetHistory(long taskId)
        {
            var result = await _mediator.Send(new TaskHistoryGetAllQuery(taskId));
            return Ok(result);
        }

        /// <summary>US-7.8: a downloadable summary for a completed task - 400 if the task isn't Completed.</summary>
        [HttpGet("{taskId}/report")]
        public async Task<IActionResult> DownloadReport(long taskId)
        {
            var result = await _mediator.Send(new TaskReportGenerateQuery(taskId));
            return File(result.Content, result.ContentType, result.FileName);
        }

    }
}
