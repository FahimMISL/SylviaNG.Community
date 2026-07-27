using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Marketplace.Commands.ConversationStart;
using SylviaNG.Community.Application.Features.Marketplace.Commands.MessageSend;
using SylviaNG.Community.Application.Features.Marketplace.Models;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ConversationGetAllPaged;
using SylviaNG.Community.Application.Features.Marketplace.Queries.ConversationGetById;
using SylviaNG.Community.Application.Features.Marketplace.Queries.MessageGetAll;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/conversation")]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ConversationController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ConversationResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var result = await _mediator.Send(new ConversationGetAllPagedQuery(employeeId, request));
            return Ok(result);
        }

        [HttpGet("{conversationId}")]
        public async Task<ActionResult<ConversationResponse>> GetById(long conversationId)
        {
            var result = await _mediator.Send(new ConversationGetByIdQuery(conversationId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Start([FromBody] ConversationStartRequest request)
        {
            var employeeId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new ConversationStartCommand(employeeId, request));
            return Ok(id);
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<ActionResult<List<MessageResponse>>> GetMessages(long conversationId)
        {
            var result = await _mediator.Send(new MessageGetAllQuery(conversationId));
            return Ok(result);
        }

        [HttpPost("{conversationId}/messages")]
        public async Task<ActionResult<long>> SendMessage(long conversationId, [FromBody] MessageSendRequest request)
        {
            var senderId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new MessageSendCommand(conversationId, senderId, request));
            return Ok(id);
        }
    }
}
