using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllForManagement
{
    public class EmployeeGetAllForManagementQuery : IRequest<PagedResult<EmployeeManagementRowResponse>>
    {
        public EmployeeFilterRequest Request { get; set; }

        public EmployeeGetAllForManagementQuery(EmployeeFilterRequest request)
        {
            Request = request;
        }
    }
}
