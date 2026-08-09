using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Branches.Commands.BranchCreate;
using SylviaNG.Community.Application.Features.Branches.Commands.BranchDelete;
using SylviaNG.Community.Application.Features.Branches.Commands.BranchUpdate;
using SylviaNG.Community.Application.Features.Branches.Models;
using SylviaNG.Community.Application.Features.Branches.Queries.BranchGetAllPaged;
using SylviaNG.Community.Application.Features.Branches.Queries.BranchGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/branch")]
    public class BranchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<BranchResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new BranchGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{branchId}")]
        public async Task<ActionResult<BranchResponse>> GetById(long branchId)
        {
            var result = await _mediator.Send(new BranchGetByIdQuery(branchId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] BranchCreateRequest request)
        {
            var id = await _mediator.Send(new BranchCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{branchId}")]
        public async Task<ActionResult> Update(long branchId, [FromBody] BranchUpdateRequest request)
        {
            await _mediator.Send(new BranchUpdateCommand(branchId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{branchId}")]
        public async Task<ActionResult> Delete(long branchId)
        {
            await _mediator.Send(new BranchDeleteCommand(branchId));
            return Ok();
        }
    }
}
