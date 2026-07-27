using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeDeactivate
{
    public class EmployeeDeactivateHandler : IRequestHandler<EmployeeDeactivateCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeDeactivateHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<Unit> Handle(EmployeeDeactivateCommand command, CancellationToken cancellationToken)
        {
            await _employeeService.DeactivateAsync(command.EmployeeId);
            return Unit.Value;
        }
    }
}
