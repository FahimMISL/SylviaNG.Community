using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetNewJoinees
{
    public class EmployeeGetNewJoineesHandler : IRequestHandler<EmployeeGetNewJoineesQuery, List<NewJoineeResponse>>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeGetNewJoineesHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<List<NewJoineeResponse>> Handle(EmployeeGetNewJoineesQuery query, CancellationToken cancellationToken)
        {
            return await _employeeService.GetNewJoineesAsync();
        }
    }
}
