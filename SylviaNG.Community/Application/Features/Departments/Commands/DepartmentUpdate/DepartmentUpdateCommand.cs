using MediatR;
using SylviaNG.Community.Application.Features.Departments.Models;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentUpdate
{
    public class DepartmentUpdateCommand : IRequest
    {
        public long DepartmentId { get; set; }
        public DepartmentUpdateRequest Request { get; set; }

        public DepartmentUpdateCommand(long departmentId, DepartmentUpdateRequest request)
        {
            DepartmentId = departmentId;
            Request = request;
        }
    }
}
