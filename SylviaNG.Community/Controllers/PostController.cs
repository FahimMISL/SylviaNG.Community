using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Posts.Commands.PostCreate;
using SylviaNG.Community.Application.Features.Posts.Commands.PostDelete;
using SylviaNG.Community.Application.Features.Posts.Commands.PostSetHidden;
using SylviaNG.Community.Application.Features.Posts.Commands.PostSetLocked;
using SylviaNG.Community.Application.Features.Posts.Commands.PostUpdate;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Features.Posts.Queries.PostGetAllPaged;
using SylviaNG.Community.Application.Features.Posts.Queries.PostGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post")]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<PostResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new PostGetAllPagedQuery(request));
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
            await _mediator.Send(new PostUpdateCommand(postId, request));
            return Ok();
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> Delete(long postId)
        {
            await _mediator.Send(new PostDeleteCommand(postId));
            return Ok();
        }

        /// <summary>
        /// Moderation action: lock/unlock a post so it no longer accepts new comments/edits.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{postId}/lock")]
        public async Task<ActionResult> SetLocked(long postId, [FromQuery] bool isLocked = true)
        {
            await _mediator.Send(new PostSetLockedCommand(postId, isLocked));
            return Ok();
        }

        /// <summary>
        /// Moderation action: hide/unhide a post from the feed.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{postId}/hide")]
        public async Task<ActionResult> SetHidden(long postId, [FromQuery] bool isHidden = true)
        {
            await _mediator.Send(new PostSetHiddenCommand(postId, isHidden));
            return Ok();
        }
    }
}
