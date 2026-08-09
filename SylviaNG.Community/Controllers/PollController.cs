using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Polls.Commands.PollCreate;
using SylviaNG.Community.Application.Features.Polls.Commands.PollVoteCast;
using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Application.Features.Polls.Queries.PollGetByPostId;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post/{postId}/poll")]
    public class PollController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PollController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PollResponse>> GetByPostId(long postId)
        {
            var result = await _mediator.Send(new PollGetByPostIdQuery(postId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create(long postId, [FromBody] PollCreateRequest request)
        {
            var id = await _mediator.Send(new PollCreateCommand(postId, request));
            return Ok(id);
        }

        [HttpPost("vote")]
        public async Task<ActionResult<long>> Vote(long postId, [FromBody] PollVoteRequest request)
        {
            var id = await _mediator.Send(new PollVoteCastCommand(postId, request));
            return Ok(id);
        }
    }
}
