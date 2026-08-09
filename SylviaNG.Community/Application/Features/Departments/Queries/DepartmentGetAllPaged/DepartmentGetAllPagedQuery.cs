using MediatR;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetAllPaged
{
    public class DepartmentGetAllPagedQuery : IRequest<PagedResult<DepartmentResponse>>
    {
        public PagedRequest Request { get; set; }

        public DepartmentGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
