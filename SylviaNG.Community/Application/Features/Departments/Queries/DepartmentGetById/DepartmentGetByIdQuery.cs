using MediatR;
using SylviaNG.Community.Application.Features.Departments.Models;

namespace SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetById
{
    public class DepartmentGetByIdQuery : IRequest<DepartmentResponse>
    {
        public long DepartmentId { get; set; }

        public DepartmentGetByIdQuery(long departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
