using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionAdd;
using SylviaNG.Community.Application.Features.PostReactions.Commands.PostReactionRemove;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Features.PostReactions.Queries.PostReactionGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post/{postId}/reactions")]
    public class PostReactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public PostReactionController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
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
            // EmployeeId is always the caller's own id - never trust a client-supplied value here,
            // otherwise any authenticated caller could react as anyone else.
            request.EmployeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new PostReactionAddCommand(postId, request));
            return Ok(result);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult> Remove(long postId, long employeeId)
        {
            // Route employeeId is only honored for HR/Admin (moderation override); everyone else
            // can only remove their own reaction regardless of what's in the route.
            var callerId = _currentUserService.EmployeeId ?? 0;
            var targetEmployeeId = _currentUserService.IsHrOrAdmin ? employeeId : callerId;
            await _mediator.Send(new PostReactionRemoveCommand(postId, targetEmployeeId));
            return Ok();
        }
    }
}
