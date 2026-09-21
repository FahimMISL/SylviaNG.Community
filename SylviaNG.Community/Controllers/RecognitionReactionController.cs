using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd;
using SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionRemove;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Queries.RecognitionReactionGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/recognition/{recognitionId}/reactions")]
    public class RecognitionReactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public RecognitionReactionController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
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
            var callerId = _currentUserService.RequireEmployeeId();
            var id = await _mediator.Send(new RecognitionReactionAddCommand(recognitionId, request, callerId));
            return Ok(id);
        }

        [HttpDelete("{employeeId}")]
        public async Task<ActionResult> Remove(long recognitionId, long employeeId)
        {
            // RecognitionService.RemoveReactionAsync only consults callerId when the caller isn't
            // HR/Admin (the self-only check); RequireEmployeeId() is only evaluated on that branch,
            // so an Admin-type caller (no Employee record) can still remove any reaction via the
            // moderation override.
            var callerId = _currentUserService.IsHrOrAdmin ? -1 : _currentUserService.RequireEmployeeId();
            await _mediator.Send(new RecognitionReactionRemoveCommand(recognitionId, employeeId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }
    }
}
