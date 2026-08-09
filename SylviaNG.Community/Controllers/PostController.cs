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
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new PostGetAllPagedQuery(request, callerId));
            return Ok(result);
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<PostResponse>> GetById(long postId)
        {
            var result = await _mediator.Send(new PostGetByIdQuery(postId));
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
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new PostUpdateCommand(postId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> Delete(long postId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
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
            var callerId = _currentUserService.EmployeeId ?? 0;
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
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new PostSetHiddenCommand(postId, isHidden, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }
    }
}
