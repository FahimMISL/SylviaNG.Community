using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdateProfile
{
    public class EmployeeUpdateProfileHandler : IRequestHandler<EmployeeUpdateProfileCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeUpdateProfileHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<Unit> Handle(EmployeeUpdateProfileCommand command, CancellationToken cancellationToken)
        {
            await _employeeService.UpdateProfileAsync(command.EmployeeId, command.Request, command.ViewerEmployeeId);
            return Unit.Value;
        }
    }
}
