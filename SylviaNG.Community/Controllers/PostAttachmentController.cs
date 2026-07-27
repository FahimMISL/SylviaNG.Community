using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.PostAttachments.Commands.PostAttachmentAdd;
using SylviaNG.Community.Application.Features.PostAttachments.Commands.PostAttachmentRemove;
using SylviaNG.Community.Application.Features.PostAttachments.Models;
using SylviaNG.Community.Application.Features.PostAttachments.Queries.PostAttachmentGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/post/{postId}/attachments")]
    public class PostAttachmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostAttachmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostAttachmentResponse>>> GetAll(long postId)
        {
            var result = await _mediator.Send(new PostAttachmentGetAllQuery(postId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Add(long postId, [FromBody] PostAttachmentAddRequest request)
        {
            var id = await _mediator.Send(new PostAttachmentAddCommand(postId, request));
            return Ok(id);
        }

        [HttpDelete("{attachmentId}")]
        public async Task<ActionResult> Remove(long postId, long attachmentId)
        {
            await _mediator.Send(new PostAttachmentRemoveCommand(postId, attachmentId));
            return Ok();
        }
    }
}
