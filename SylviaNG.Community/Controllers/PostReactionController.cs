using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionAdd;
using SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionRemove;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Features.PostReactions.Queries.PostReactionGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post/{postId}/reactions")]
    public class PostReactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostReactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostReactionResponse>>> GetAll(long postId)
        {
            var result = await _mediator.Send(new PostReactionGetAllQuery(postId));
            return Ok(result);
        }

        /// <summary>
        /// Adds a reaction, or toggles it off if the same employee/type combination
        /// already exists. Returns null content when toggled off.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PostReactionResponse?>> Add(long postId, [FromBody] PostReactionAddRequest request)
        {
            var result = await _mediator.Send(new PostReactionAddCommand(postId, request));
            return Ok(result);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult> Remove(long postId, long employeeId)
        {
            await _mediator.Send(new PostReactionRemoveCommand(postId, employeeId));
            return Ok();
        }
    }
}
