using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateCoverPhoto
{
    public class EmployeeUpdateCoverPhotoHandler : IRequestHandler<EmployeeUpdateCoverPhotoCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeUpdateCoverPhotoHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<Unit> Handle(EmployeeUpdateCoverPhotoCommand command, CancellationToken cancellationToken)
        {
            await _employeeService.UpdateCoverPhotoAsync(command.EmployeeId, command.Request.StoragePath, command.ViewerEmployeeId);
            return Unit.Value;
        }
    }
}
