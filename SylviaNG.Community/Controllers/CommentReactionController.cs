using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionAdd;
using SylviaNG.Community.Application.Features.CommentReactions.Commands.CommentReactionRemove;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Features.CommentReactions.Queries.CommentReactionGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/comment/{commentId}/reactions")]
    public class CommentReactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public CommentReactionController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
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
            // EmployeeId is always the caller's own id - never trust a client-supplied value here,
            // otherwise any authenticated caller could react as anyone else.
            request.EmployeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new CommentReactionAddCommand(commentId, request));
            return Ok(result);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult> Remove(long commentId, long employeeId)
        {
            // Route employeeId is only honored for HR/Admin (moderation override); everyone else
            // can only remove their own reaction regardless of what's in the route.
            var callerId = _currentUserService.EmployeeId ?? 0;
            var targetEmployeeId = _currentUserService.IsHrOrAdmin ? employeeId : callerId;
            await _mediator.Send(new CommentReactionRemoveCommand(commentId, targetEmployeeId));
            return Ok();
        }
    }
}
