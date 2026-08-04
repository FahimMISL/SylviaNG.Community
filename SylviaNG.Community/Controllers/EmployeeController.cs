using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeCreate;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeDeactivate;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateCoverPhoto;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdatePhoto;
using SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllForManagement;
using SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllPaged;
using SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetById;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Controllers
{
    [ApiController]
    [Route("community/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public EmployeeController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Browse the employee directory (US-1.1/1.2). Always restricted to active employees.
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<EmployeeDirectoryCardResponse>>> GetDirectoryPaged([FromQuery] EmployeeFilterRequest request)
        {
            var result = await _mediator.Send(new EmployeeGetAllPagedQuery(request));
            return Ok(result);
        }

        /// <summary>
        /// Full employee list including inactive, for HR/Admin User Management (US-1.7).
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpGet("management/paged")]
        public async Task<ActionResult<PagedResult<EmployeeManagementRowResponse>>> GetManagementPaged([FromQuery] EmployeeFilterRequest request)
        {
            var result = await _mediator.Send(new EmployeeGetAllForManagementQuery(request));
            return Ok(result);
        }

        /// <summary>
        /// Full profile (US-1.3 colleague view / US-1.4 my-profile view). Private contact
        /// fields are hidden unless the viewer is HR/Admin or the profile owner.
        /// </summary>
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(long employeeId)
        {
            var result = await _mediator.Send(new EmployeeGetByIdQuery(employeeId, _currentUserService.EmployeeId, _currentUserService.IsHrOrAdmin));
            return Ok(result);
        }

        /// <summary>
        /// Add a new employee (US-1.6).
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpPost]
        public async Task<ActionResult<long>> Create([FromBody] EmployeeCreateRequest request)
        {
            var id = await _mediator.Send(new EmployeeCreateCommand(request));
            return Ok(id);
        }

        /// <summary>
        /// Edit my own profile - bio, skills, interests, contact-field privacy (US-1.5).
        /// </summary>
        [HttpPut("{employeeId}/profile")]
        public async Task<ActionResult> UpdateProfile(long employeeId, [FromBody] EmployeeUpdateProfileRequest request)
        {
            await _mediator.Send(new EmployeeUpdateProfileCommand(employeeId, request, _currentUserService.EmployeeId));
            return Ok();
        }

        /// <summary>
        /// Set my own profile photo. StoragePath must come from a prior POST community/file-upload
        /// call (module "employee-photo").
        /// </summary>
        [HttpPut("{employeeId}/photo")]
        public async Task<ActionResult> UpdatePhoto(long employeeId, [FromBody] EmployeeUpdatePhotoRequest request)
        {
            await _mediator.Send(new EmployeeUpdatePhotoCommand(employeeId, request, _currentUserService.EmployeeId));
            return Ok();
        }

        /// <summary>
        /// Set my own cover photo. StoragePath must come from a prior POST community/file-upload
        /// call (module "employee-cover").
        /// </summary>
        [HttpPut("{employeeId}/cover-photo")]
        public async Task<ActionResult> UpdateCoverPhoto(long employeeId, [FromBody] EmployeeUpdateCoverPhotoRequest request)
        {
            await _mediator.Send(new EmployeeUpdateCoverPhotoCommand(employeeId, request, _currentUserService.EmployeeId));
            return Ok();
        }

        /// <summary>
        /// Deactivate an employee (US-1.8).
        /// </summary>
        [Authorize(Policy = "HRAdminOnly")]
        [HttpPut("{employeeId}/deactivate")]
        public async Task<ActionResult> Deactivate(long employeeId)
        {
            await _mediator.Send(new EmployeeDeactivateCommand(employeeId));
            return Ok();
        }
    }
}
