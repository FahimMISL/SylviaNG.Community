using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdate
{
    public class EmployeeUpdateHandler : IRequestHandler<EmployeeUpdateCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeUpdateHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<Unit> Handle(EmployeeUpdateCommand command, CancellationToken cancellationToken)
        {
            await _employeeService.UpdateAsync(command.EmployeeId, command.Request);
            return Unit.Value;
        }
    }
}
