using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Interests.Commands.InterestCreate;
using SylviaNG.Community.Application.Features.Interests.Commands.InterestDelete;
using SylviaNG.Community.Application.Features.Interests.Models;
using SylviaNG.Community.Application.Features.Interests.Queries.InterestGetAll;
using SylviaNG.Community.Application.Features.Interests.Queries.InterestGetAllPaged;
using SylviaNG.Community.Application.Features.Interests.Queries.InterestGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/interest")]
    public class InterestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InterestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InterestResponse>>> GetAll()
        {
            var result = await _mediator.Send(new InterestGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<InterestResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new InterestGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{interestId}")]
        public async Task<ActionResult<InterestResponse>> GetById(long interestId)
        {
            var result = await _mediator.Send(new InterestGetByIdQuery(interestId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] InterestCreateRequest request)
        {
            var id = await _mediator.Send(new InterestCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{interestId}")]
        public async Task<ActionResult> Delete(long interestId)
        {
            await _mediator.Send(new InterestDeleteCommand(interestId));
            return Ok();
        }
    }
}
