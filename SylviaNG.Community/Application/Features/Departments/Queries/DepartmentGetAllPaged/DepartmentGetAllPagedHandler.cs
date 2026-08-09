using MediatR;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetAllPaged
{
    public class DepartmentGetAllPagedHandler : IRequestHandler<DepartmentGetAllPagedQuery, PagedResult<DepartmentResponse>>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentGetAllPagedHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<PagedResult<DepartmentResponse>> Handle(DepartmentGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _departmentService.GetPaginatedAsync(query.Request);
        }
    }
}
