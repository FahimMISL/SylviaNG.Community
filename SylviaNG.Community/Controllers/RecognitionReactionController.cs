using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd;
using SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionRemove;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Queries.RecognitionReactionGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/recognition/{recognitionId}/reactions")]
    public class RecognitionReactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecognitionReactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RecognitionReactionResponse>>> GetAll(long recognitionId)
        {
            var result = await _mediator.Send(new RecognitionReactionGetAllQuery(recognitionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Add(long recognitionId, [FromBody] RecognitionReactionAddRequest request)
        {
            var id = await _mediator.Send(new RecognitionReactionAddCommand(recognitionId, request));
            return Ok(id);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult> Remove(long recognitionId, long employeeId)
        {
            await _mediator.Send(new RecognitionReactionRemoveCommand(recognitionId, employeeId));
            return Ok();
        }
    }
}
