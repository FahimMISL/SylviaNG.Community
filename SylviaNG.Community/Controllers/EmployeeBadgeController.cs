using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.EmployeeBadges.Commands.EmployeeBadgeAward;
using SylviaNG.Community.Application.Features.EmployeeBadges.Models;
using SylviaNG.Community.Application.Features.EmployeeBadges.Queries.EmployeeBadgeGetAll;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/employee/{employeeId}/badges")]
    public class EmployeeBadgeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeBadgeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeBadgeResponse>>> GetAll(long employeeId)
        {
            var result = await _mediator.Send(new EmployeeBadgeGetAllQuery(employeeId));
            return Ok(result);
        }

        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Award(long employeeId, [FromBody] EmployeeBadgeAwardRequest request)
        {
            var id = await _mediator.Send(new EmployeeBadgeAwardCommand(employeeId, request));
            return Ok(id);
        }
    }
}
