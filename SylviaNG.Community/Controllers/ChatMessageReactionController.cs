using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReact;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/chat/messages/{messageId}/reactions")]
    public class ChatMessageReactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ChatMessageReactionController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Adds a reaction, or toggles it off if the caller already reacted with the same
        /// type. Returns null content when toggled off - mirrors PostReactionController.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ChatMessageReactionResponse?>> React(long messageId, [FromBody] ChatMessageReactionRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ChatMessageReactCommand(messageId, request.ReactionType, callerId));
            return Ok(result);
        }
    }
}
