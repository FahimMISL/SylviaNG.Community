using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdatePhoto
{
    public class EmployeeUpdatePhotoHandler : IRequestHandler<EmployeeUpdatePhotoCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeUpdatePhotoHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<Unit> Handle(EmployeeUpdatePhotoCommand command, CancellationToken cancellationToken)
        {
            await _employeeService.UpdatePhotoAsync(command.EmployeeId, command.Request.StoragePath, command.ViewerEmployeeId);
            return Unit.Value;
        }
    }
}
