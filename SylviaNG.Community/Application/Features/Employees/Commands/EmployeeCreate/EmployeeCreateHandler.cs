using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeCreate
{
    public class EmployeeCreateHandler : IRequestHandler<EmployeeCreateCommand, long>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeCreateHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<long> Handle(EmployeeCreateCommand command, CancellationToken cancellationToken)
        {
            return await _employeeService.CreateAsync(command.Request);
        }
    }
}
