using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Commands.EmployeeCreate
{
    public class EmployeeCreateCommand : IRequest<long>
    {
        public EmployeeCreateRequest Request { get; set; }

        public EmployeeCreateCommand(EmployeeCreateRequest request)
        {
            Request = request;
        }
    }
}
