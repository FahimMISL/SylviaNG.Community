using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetTodayEvents
{
    public class EmployeeGetTodayEventsHandler : IRequestHandler<EmployeeGetTodayEventsQuery, List<TodayEventResponse>>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeGetTodayEventsHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<List<TodayEventResponse>> Handle(EmployeeGetTodayEventsQuery query, CancellationToken cancellationToken)
        {
            return await _employeeService.GetTodayEventsAsync();
        }
    }
}
