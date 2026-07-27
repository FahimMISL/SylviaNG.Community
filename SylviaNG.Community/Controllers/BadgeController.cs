using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Badges.Commands.BadgeCreate;
using SylviaNG.Community.Application.Features.Badges.Commands.BadgeDelete;
using SylviaNG.Community.Application.Features.Badges.Models;
using SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetAll;
using SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetAllPaged;
using SylviaNG.Community.Application.Features.Badges.Queries.BadgeGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/badge")]
    public class BadgeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BadgeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<BadgeResponse>>> GetAll()
        {
            var result = await _mediator.Send(new BadgeGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<BadgeResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new BadgeGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{badgeId}")]
        public async Task<ActionResult<BadgeResponse>> GetById(long badgeId)
        {
            var result = await _mediator.Send(new BadgeGetByIdQuery(badgeId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] BadgeCreateRequest request)
        {
            var id = await _mediator.Send(new BadgeCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{badgeId}")]
        public async Task<ActionResult> Delete(long badgeId)
        {
            await _mediator.Send(new BadgeDeleteCommand(badgeId));
            return Ok();
        }
    }
}
