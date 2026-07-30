using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentAdd;
using SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentDelete;
using SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentUpdate;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Features.PostComments.Queries.PostCommentGetAll;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post/{postId}/comments")]
    public class PostCommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public PostCommentController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Returns a flat list of comments for the post; ParentCommentId lets the
        /// client reconstruct the reply thread.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PostCommentResponse>>> GetAll(long postId)
        {
            var result = await _mediator.Send(new PostCommentGetAllQuery(postId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Add(long postId, [FromBody] PostCommentAddRequest request)
        {
            var id = await _mediator.Send(new PostCommentAddCommand(postId, request));
            return Ok(id);
        }

        [HttpPut("{commentId}")]
        public async Task<ActionResult> Update(long postId, long commentId, [FromBody] PostCommentUpdateRequest request)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new PostCommentUpdateCommand(postId, commentId, request, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }

        [HttpDelete("{commentId}")]
        public async Task<ActionResult> Delete(long postId, long commentId)
        {
            var callerId = _currentUserService.EmployeeId ?? 0;
            await _mediator.Send(new PostCommentDeleteCommand(postId, commentId, callerId, _currentUserService.IsHrOrAdmin));
            return Ok();
        }
    }
}
