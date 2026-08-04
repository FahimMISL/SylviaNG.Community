using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.RecognitionComments.Commands.RecognitionCommentAdd;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionComments.Queries.RecognitionCommentGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/recognition/{recognitionId}/comments")]
    public class RecognitionCommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public RecognitionCommentController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
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
            var callerId = _currentUserService.EmployeeId ?? 0;
            var id = await _mediator.Send(new RecognitionCommentAddCommand(recognitionId, request, callerId));
            return Ok(id);
        }
    }
}
