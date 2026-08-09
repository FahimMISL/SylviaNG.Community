using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllPaged
{
    public class EmployeeGetAllPagedQuery : IRequest<PagedResult<EmployeeDirectoryCardResponse>>
    {
        public EmployeeFilterRequest Request { get; set; }

        public EmployeeGetAllPagedQuery(EmployeeFilterRequest request)
        {
            Request = request;
        }
    }
}
