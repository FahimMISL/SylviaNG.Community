using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.RecognitionComments.Commands.RecognitionCommentAdd;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionComments.Queries.RecognitionCommentGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/recognition/{recognitionId}/comments")]
    public class RecognitionCommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecognitionCommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RecognitionCommentResponse>>> GetAll(long recognitionId)
        {
            var result = await _mediator.Send(new RecognitionCommentGetAllQuery(recognitionId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Add(long recognitionId, [FromBody] RecognitionCommentAddRequest request)
        {
            var id = await _mediator.Send(new RecognitionCommentAddCommand(recognitionId, request));
            return Ok(id);
        }
    }
}
