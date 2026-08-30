using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Features.ChatReports.Commands.ChatReportResolve;
using SylviaNG.Community.Application.Features.ChatReports.Models;
using SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportConversationGetForModeration;
using SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportGetAllPaged;
using SylviaNG.Community.Application.Features.ChatReports.Queries.ChatReportMessagesGetPagedForModeration;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/chat-report")]
    public class ChatReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Moderation queue. Report creation stays on ChatMessageController.Report - this
        /// controller only covers the HR/Admin-facing review workflow.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ChatReportQueueItemResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ChatReportGetAllPagedQuery(request));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{reportId}/resolve")]
        public async Task<ActionResult> Resolve(long reportId, [FromBody] ChatReportResolveRequest request)
        {
            await _mediator.Send(new ChatReportResolveCommand(reportId, request));
            return Ok();
        }

        /// <summary>
        /// HR/Admin-only: conversation metadata for a reported conversation, bypassing the normal
        /// participant-only access check so a moderator can review a report they aren't a party to.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("conversations/{conversationId}")]
        public async Task<ActionResult<ChatConversationResponse>> GetConversationForModeration(long conversationId)
        {
            var result = await _mediator.Send(new ChatReportConversationGetForModerationQuery(conversationId));
            return Ok(result);
        }

        /// <summary>HR/Admin-only: the full surrounding thread for a reported conversation, bypassing the normal participant-only access check.</summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("conversations/{conversationId}/messages/paged")]
        public async Task<ActionResult<PagedResult<ChatMessageResponse>>> GetMessagesForModeration(long conversationId, [FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new ChatReportMessagesGetPagedForModerationQuery(conversationId, request));
            return Ok(result);
        }
    }
}
