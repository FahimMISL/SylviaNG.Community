using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetById
{
    public class EmployeeGetByIdHandler : IRequestHandler<EmployeeGetByIdQuery, EmployeeResponse>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeGetByIdHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<EmployeeResponse> Handle(EmployeeGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _employeeService.GetByIdAsync(query.EmployeeId, query.ViewerEmployeeId, query.ViewerIsHrAdmin);
        }
    }
}
