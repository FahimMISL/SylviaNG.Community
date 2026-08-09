using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Skills.Commands.SkillCreate;
using SylviaNG.Community.Application.Features.Skills.Commands.SkillDelete;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAll;
using SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAllPaged;
using SylviaNG.Community.Application.Features.Skills.Queries.SkillGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/skill")]
    public class SkillController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SkillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillResponse>>> GetAll()
        {
            var result = await _mediator.Send(new SkillGetAllQuery());
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SkillResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new SkillGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{skillId}")]
        public async Task<ActionResult<SkillResponse>> GetById(long skillId)
        {
            var result = await _mediator.Send(new SkillGetByIdQuery(skillId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] SkillCreateRequest request)
        {
            var id = await _mediator.Send(new SkillCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{skillId}")]
        public async Task<ActionResult> Delete(long skillId)
        {
            await _mediator.Send(new SkillDeleteCommand(skillId));
            return Ok();
        }
    }
}
