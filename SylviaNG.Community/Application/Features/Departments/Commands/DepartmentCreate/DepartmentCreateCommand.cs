using MediatR;
using SylviaNG.Community.Application.Features.Departments.Models;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentCreate
{
    public class DepartmentCreateCommand : IRequest<long>
    {
        public DepartmentCreateRequest Request { get; set; }

        public DepartmentCreateCommand(DepartmentCreateRequest request)
        {
            Request = request;
        }
    }
}
