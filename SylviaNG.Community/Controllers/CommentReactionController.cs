using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionAdd;
using SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionRemove;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Features.CommentReactions.Queries.CommentReactionGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/comment/{commentId}/reactions")]
    public class CommentReactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentReactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentReactionResponse>>> GetAll(long commentId)
        {
            var result = await _mediator.Send(new CommentReactionGetAllQuery(commentId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CommentReactionResponse?>> Add(long commentId, [FromBody] CommentReactionAddRequest request)
        {
            var result = await _mediator.Send(new CommentReactionAddCommand(commentId, request));
            return Ok(result);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult> Remove(long commentId, long employeeId)
        {
            await _mediator.Send(new CommentReactionRemoveCommand(commentId, employeeId));
            return Ok();
        }
    }
}
