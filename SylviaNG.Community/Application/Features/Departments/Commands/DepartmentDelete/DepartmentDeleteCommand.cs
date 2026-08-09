using MediatR;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentDelete
{
    public class DepartmentDeleteCommand : IRequest
    {
        public long DepartmentId { get; set; }

        public DepartmentDeleteCommand(long departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
