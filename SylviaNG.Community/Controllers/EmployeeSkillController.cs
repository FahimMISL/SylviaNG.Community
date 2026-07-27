using MediatR;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillAssign;
using SylviaNG.Community.Application.Features.EmployeeSkills.Commands.EmployeeSkillRemove;
using SylviaNG.Community.Application.Features.EmployeeSkills.Models;
using SylviaNG.Community.Application.Features.EmployeeSkills.Queries.EmployeeSkillGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/employee/{employeeId}/skills")]
    public class EmployeeSkillController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeSkillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeSkillResponse>>> GetAll(long employeeId)
        {
            var result = await _mediator.Send(new EmployeeSkillGetAllQuery(employeeId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<long>> Assign(long employeeId, [FromBody] EmployeeSkillAssignRequest request)
        {
            var id = await _mediator.Send(new EmployeeSkillAssignCommand(employeeId, request));
            return Ok(id);
        }

        [HttpDelete("{skillId}")]
        public async Task<ActionResult> Remove(long employeeId, long skillId)
        {
            await _mediator.Send(new EmployeeSkillRemoveCommand(employeeId, skillId));
            return Ok();
        }
    }
}
