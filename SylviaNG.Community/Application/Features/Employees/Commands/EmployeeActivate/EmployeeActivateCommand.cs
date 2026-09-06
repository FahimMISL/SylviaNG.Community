using MediatR;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeActivate
{
    public class EmployeeActivateCommand : IRequest<Unit>
    {
        public long EmployeeId { get; set; }

        public EmployeeActivateCommand(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
