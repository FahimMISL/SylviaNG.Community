using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageDelete;
using SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageForward;
using SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReport;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/chat/messages/{messageId}")]
    public class ChatMessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ChatMessageController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>"Remove for Everyone" - sender-only. The message stays in the thread as a tombstone for every participant.</summary>
        [HttpDelete]
        public async Task<ActionResult> Delete(long messageId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatMessageDeleteCommand(messageId, callerId));
            return Ok();
        }

        /// <summary>Copies this message into one or more of the caller's own other conversations.</summary>
        [HttpPost("forward")]
        public async Task<ActionResult> Forward(long messageId, [FromBody] ChatMessageForwardRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatMessageForwardCommand(messageId, request, callerId));
            return Ok();
        }

        /// <summary>Files a moderation report against this message.</summary>
        [HttpPost("report")]
        public async Task<ActionResult> Report(long messageId, [FromBody] ChatMessageReportRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatMessageReportCommand(messageId, request, callerId));
            return Ok();
        }
    }
}
