using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetAllPaged
{
    public class EmployeeGetAllPagedHandler : IRequestHandler<EmployeeGetAllPagedQuery, PagedResult<EmployeeDirectoryCardResponse>>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeGetAllPagedHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<PagedResult<EmployeeDirectoryCardResponse>> Handle(EmployeeGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _employeeService.GetDirectoryPaginatedAsync(query.Request);
        }
    }
}
