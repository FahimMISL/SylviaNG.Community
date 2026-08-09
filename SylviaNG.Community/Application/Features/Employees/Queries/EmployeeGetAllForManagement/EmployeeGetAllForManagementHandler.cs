using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllForManagement
{
    public class EmployeeGetAllForManagementHandler : IRequestHandler<EmployeeGetAllForManagementQuery, PagedResult<EmployeeManagementRowResponse>>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeGetAllForManagementHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<PagedResult<EmployeeManagementRowResponse>> Handle(EmployeeGetAllForManagementQuery query, CancellationToken cancellationToken)
        {
            return await _employeeService.GetManagementPaginatedAsync(query.Request);
        }
    }
}
