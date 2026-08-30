using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationCreate;
using SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationMarkRead;
using SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetMuted;
using SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationSetPinned;
using SylviaNG.Community.Application.Features.ChatConversations.Commands.ChatConversationUpdateGroup;
using SylviaNG.Community.Application.Features.ChatConversations.Models;
using SylviaNG.Community.Application.Features.ChatConversations.Queries.ChatConversationGetAllPaged;
using SylviaNG.Community.Application.Features.ChatConversations.Queries.ChatConversationGetById;
using SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageSend;
using SylviaNG.Community.Application.Features.ChatMessages.Models;
using SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageGetAllPaged;
using SylviaNG.Community.Application.Features.ChatMessages.Queries.ChatMessageSearch;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/chat")]
    public class ChatConversationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ChatConversationController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// My conversation inbox - pinned first, then most recent activity.
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ChatConversationSummaryResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ChatConversationGetAllPagedQuery(request, callerId));
            return Ok(result);
        }

        [HttpGet("{conversationId}")]
        public async Task<ActionResult<ChatConversationResponse>> GetById(long conversationId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ChatConversationGetByIdQuery(conversationId, callerId));
            return Ok(result);
        }

        /// <summary>
        /// Starts a Direct (1:1) or Group conversation. Starting a Direct conversation that
        /// already exists between the two employees returns the existing one instead of
        /// creating a duplicate.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] ChatConversationCreateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new ChatConversationCreateCommand(request, callerId));
            return Ok(id);
        }

        [HttpGet("{conversationId}/messages/paged")]
        public async Task<ActionResult<PagedResult<ChatMessageResponse>>> GetMessagesPaged(long conversationId, [FromQuery] PagedRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ChatMessageGetAllPagedQuery(conversationId, request, callerId));
            return Ok(result);
        }

        [HttpPost("{conversationId}/messages")]
        public async Task<ActionResult<ChatMessageResponse>> SendMessage(long conversationId, [FromBody] ChatMessageSendRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ChatMessageSendCommand(conversationId, request, callerId));
            return Ok(result);
        }

        /// <summary>
        /// Body-text search across every conversation the caller is an active participant of.
        /// Placed under /messages/search (not nested under a conversation id) since it spans
        /// conversations - see MessengerService in the frontend for how a result routes back
        /// to its own conversation.
        /// </summary>
        [HttpGet("messages/search")]
        public async Task<ActionResult<PagedResult<ChatMessageResponse>>> SearchMessages([FromQuery] string searchTerm, [FromQuery] PagedRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ChatMessageSearchQuery(searchTerm, request, callerId));
            return Ok(result);
        }

        /// <summary>US-12.7: advances the caller's own read watermark to now.</summary>
        [HttpPut("{conversationId}/read")]
        public async Task<ActionResult> MarkRead(long conversationId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatConversationMarkReadCommand(conversationId, callerId));
            return Ok();
        }

        /// <summary>US-12.13: mutes/unmutes the caller's own Notification Center entries for this conversation.</summary>
        [HttpPut("{conversationId}/mute")]
        public async Task<ActionResult> SetMuted(long conversationId, [FromBody] ChatConversationMuteRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatConversationSetMutedCommand(conversationId, callerId, request.IsMuted));
            return Ok();
        }

        /// <summary>US-12.13: pins/unpins this conversation to the top of the caller's own inbox.</summary>
        [HttpPut("{conversationId}/pin")]
        public async Task<ActionResult> SetPinned(long conversationId, [FromBody] ChatConversationPinRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatConversationSetPinnedCommand(conversationId, callerId, request.IsPinned));
            return Ok();
        }

        /// <summary>Admin-only: updates a group conversation's title and/or photo (GroupAvatarFileId from a prior community/file-upload call).</summary>
        [HttpPut("{conversationId}/group")]
        public async Task<ActionResult> UpdateGroup(long conversationId, [FromBody] ChatConversationUpdateGroupRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new ChatConversationUpdateGroupCommand(conversationId, request, callerId));
            return Ok();
        }
    }
}
