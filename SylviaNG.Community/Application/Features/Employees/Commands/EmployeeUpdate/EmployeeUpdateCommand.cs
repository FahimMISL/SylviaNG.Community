using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeUpdate
{
    public class EmployeeUpdateCommand : IRequest<Unit>
    {
        public long EmployeeId { get; set; }
        public EmployeeUpdateRequest Request { get; set; }

        public EmployeeUpdateCommand(long employeeId, EmployeeUpdateRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
