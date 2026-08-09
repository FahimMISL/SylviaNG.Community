using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentAdd;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskAttachmentRemove;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskCommentAdd;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskCreate;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskDelete;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagAssign;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskTagRemove;
using SylviaNG.Community.Application.Features.Tasks.Commands.TaskUpdate;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskAttachmentGetAll;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskCommentGetAll;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetAllPaged;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskGetById;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskHistoryGetAll;
using SylviaNG.Community.Application.Features.Tasks.Queries.TaskTagGetAllForTask;
using SylviaNG.Community.Application.Features.TaskTags.Models;
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

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<TaskResponse>>> GetPaged([FromQuery] TaskFilterRequest request)
        {
            var result = await _mediator.Send(new TaskGetAllPagedQuery(request));
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
            var id = await _mediator.Send(new TaskCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{taskId}")]
        public async Task<ActionResult> Update(long taskId, [FromBody] TaskUpdateRequest request)
        {
            await _mediator.Send(new TaskUpdateCommand(taskId, request, _currentUserService.EmployeeId));
            return Ok();
        }

        [HttpDelete("{taskId}")]
        public async Task<ActionResult> Delete(long taskId)
        {
            await _mediator.Send(new TaskDeleteCommand(taskId));
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
            var id = await _mediator.Send(new TaskCommentAddCommand(taskId, request));
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
            var id = await _mediator.Send(new TaskAttachmentAddCommand(taskId, request));
            return Ok(id);
        }

        [HttpDelete("{taskId}/attachments/{attachmentId}")]
        public async Task<ActionResult> RemoveAttachment(long taskId, long attachmentId)
        {
            await _mediator.Send(new TaskAttachmentRemoveCommand(taskId, attachmentId));
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

        [HttpGet("{taskId}/tags")]
        public async Task<ActionResult<List<TaskTagResponse>>> GetTags(long taskId)
        {
            var result = await _mediator.Send(new TaskTagGetAllForTaskQuery(taskId));
            return Ok(result);
        }

        [HttpPost("{taskId}/tags")]
        public async Task<ActionResult<long>> AssignTag(long taskId, [FromBody] TaskTagAssignRequest request)
        {
            var id = await _mediator.Send(new TaskTagAssignCommand(taskId, request));
            return Ok(id);
        }

        [HttpDelete("{taskId}/tags/{tagId}")]
        public async Task<ActionResult> RemoveTag(long taskId, long tagId)
        {
            await _mediator.Send(new TaskTagRemoveCommand(taskId, tagId));
            return Ok();
        }
    }
}
