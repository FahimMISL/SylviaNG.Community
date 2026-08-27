using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialCreate;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Commands.EmployeeCredentialResetPassword;
using SylviaNG.Community.Application.Features.EmployeeCredentials.Models;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/employee/{employeeId}/credential")]
    public class EmployeeCredentialController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeCredentialController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Provisions a real Keycloak login account for an existing employee (username + password,
        /// usable to log in immediately - see KeycloakAdminClient.CreateUserAsync for why this
        /// can't be a forced-change-at-first-login credential) and assigns a Keycloak realm role.
        /// HR/Admin only.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<EmployeeCredentialResponse>> Create(long employeeId, [FromBody] EmployeeCredentialCreateRequest request)
        {
            request.EmployeeId = employeeId;
            var result = await _mediator.Send(new EmployeeCredentialCreateCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Sets a new password for an employee who already has a Keycloak login, usable
        /// immediately (same reasoning as the initial grant). HR/Admin only.
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("reset-password")]
        public async Task<ActionResult> ResetPassword(long employeeId, [FromBody] EmployeeCredentialResetPasswordRequest request)
        {
            await _mediator.Send(new EmployeeCredentialResetPasswordCommand(employeeId, request));
            return Ok();
        }
    }
}
