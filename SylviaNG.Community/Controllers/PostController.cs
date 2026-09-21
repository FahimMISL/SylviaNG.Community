using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Posts.Commands.PostCreate;
using SylviaNG.Community.Application.Features.Posts.Commands.PostDelete;
using SylviaNG.Community.Application.Features.Posts.Commands.PostSetHidden;
using SylviaNG.Community.Application.Features.Posts.Commands.PostSetLocked;
using SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Features.Posts.Queries.PostGetAllPaged;
using SylviaNG.Community.Application.Features.Posts.Queries.PostGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post")]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public PostController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<PostResponse>>> GetPaged([FromQuery] PostFilterRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? -1;
            var result = await _mediator.Send(new PostGetAllPagedQuery(request, callerId));
            return Ok(result);
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<PostResponse>> GetById(long postId)
        {
            var callerId = _currentUserService.EmployeeId ?? -1;
            var result = await _mediator.Send(new PostGetByIdQuery(postId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] PostCreateRequest request)
        {
            var id = await _mediator.Send(new PostCreateCommand(request));
            return Ok(id);
        }

        [HttpPut("{postId}")]
        public async Task<ActionResult> Update(long postId, [FromBody] PostUpdateRequest request)
        {
            // PostService.UpdateAsync only consults callerId when the caller isn't HR/Admin (the
            // author-only check); RequireEmployeeId() is only evaluated on that branch, so an
            // Admin-type caller (no Employee record) can still edit any post via the moderation override.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new PostUpdateCommand(postId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> Delete(long postId)
        {
            // Same rationale as Update above: RequireEmployeeId() is only evaluated when the caller
            // isn't HR/Admin, preserving the moderation-delete override for an Admin-type caller.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new PostDeleteCommand(postId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        /// <summary>
        /// Moderation action: lock/unlock a post so it no longer accepts new comments/edits.
        /// HR/Admin can do this for any post; a group post's own Creator/GroupAdmin/Contributor
        /// can also do it, scoped to that group - enforced in PostService, not here.
        /// </summary>
        [HttpPut("{postId}/lock")]
        public async Task<ActionResult> SetLocked(long postId, [FromQuery] bool isLocked = true)
        {
            // PostService.SetLockedAsync only consults callerId for the group-moderator check when
            // the caller isn't HR/Admin; RequireEmployeeId() is only evaluated on that branch, so an
            // Admin-type caller (no Employee record) can still lock/unlock any post.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new PostSetLockedCommand(postId, isLocked, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        /// <summary>
        /// Moderation action: hide/unhide a post from the feed. Same authorization rule as
        /// SetLocked above.
        /// </summary>
        [HttpPut("{postId}/hide")]
        public async Task<ActionResult> SetHidden(long postId, [FromQuery] bool isHidden = true)
        {
            // Same rationale as SetLocked above.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new PostSetHiddenCommand(postId, isHidden, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }
    }
}
