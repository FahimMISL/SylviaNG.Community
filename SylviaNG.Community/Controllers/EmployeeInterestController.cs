using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestAssign;
using SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestRemove;
using SylviaNG.Community.Application.Features.EmployeeInterests.Models;
using SylviaNG.Community.Application.Features.EmployeeInterests.Queries.EmployeeInterestGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/employee/{employeeId}/interests")]
    public class EmployeeInterestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeInterestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeInterestResponse>>> GetAll(long employeeId)
        {
            var result = await _mediator.Send(new EmployeeInterestGetAllQuery(employeeId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Assign(long employeeId, [FromBody] EmployeeInterestAssignRequest request)
        {
            var id = await _mediator.Send(new EmployeeInterestAssignCommand(employeeId, request));
            return Ok(id);
        }

        [HttpDelete("{interestId}")]
        public async Task<ActionResult> Remove(long employeeId, long interestId)
        {
            await _mediator.Send(new EmployeeInterestRemoveCommand(employeeId, interestId));
            return Ok();
        }
    }
}
