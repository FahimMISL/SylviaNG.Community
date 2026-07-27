using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetAllPaged;
using SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/recognition")]
    public class RecognitionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecognitionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<RecognitionResponse>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] long? senderId,
            [FromQuery] long? recipientId)
        {
            var result = await _mediator.Send(new RecognitionGetAllPagedQuery(request, senderId, recipientId));
            return Ok(result);
        }

        [HttpGet("{recognitionId}")]
        public async Task<ActionResult<RecognitionResponse>> GetById(long recognitionId)
        {
            var result = await _mediator.Send(new RecognitionGetByIdQuery(recognitionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] RecognitionCreateRequest request)
        {
            var id = await _mediator.Send(new RecognitionCreateCommand(request));
            return Ok(id);
        }
    }
}
