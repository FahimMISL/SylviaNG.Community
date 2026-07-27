using MediatR;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeDeactivate
{
    public class EmployeeDeactivateCommand : IRequest<Unit>
    {
        public long EmployeeId { get; set; }

        public EmployeeDeactivateCommand(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
