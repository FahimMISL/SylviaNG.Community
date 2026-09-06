using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeActivate
{
    public class EmployeeActivateHandler : IRequestHandler<EmployeeActivateCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeActivateHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<Unit> Handle(EmployeeActivateCommand command, CancellationToken cancellationToken)
        {
            await _employeeService.ActivateAsync(command.EmployeeId);
            return Unit.Value;
        }
    }
}
