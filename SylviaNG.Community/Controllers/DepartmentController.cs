using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Departments.Commands.DepartmentCreate;
using SylviaNG.Community.Application.Features.Departments.Commands.DepartmentDelete;
using SylviaNG.Community.Application.Features.Departments.Commands.DepartmentUpdate;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetAllPaged;
using SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetById;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<DepartmentResponse>>> GetPaged([FromQuery] PagedRequest request)
        {
            var result = await _mediator.Send(new DepartmentGetAllPagedQuery(request));
            return Ok(result);
        }

        [HttpGet("{departmentId}")]
        public async Task<ActionResult<DepartmentResponse>> GetById(long departmentId)
        {
            var result = await _mediator.Send(new DepartmentGetByIdQuery(departmentId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] DepartmentCreateRequest request)
        {
            var id = await _mediator.Send(new DepartmentCreateCommand(request));
            return Ok(id);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{departmentId}")]
        public async Task<ActionResult> Update(long departmentId, [FromBody] DepartmentUpdateRequest request)
        {
            await _mediator.Send(new DepartmentUpdateCommand(departmentId, request));
            return Ok();
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpDelete("{departmentId}")]
        public async Task<ActionResult> Delete(long departmentId)
        {
            await _mediator.Send(new DepartmentDeleteCommand(departmentId));
            return Ok();
        }
    }
}
