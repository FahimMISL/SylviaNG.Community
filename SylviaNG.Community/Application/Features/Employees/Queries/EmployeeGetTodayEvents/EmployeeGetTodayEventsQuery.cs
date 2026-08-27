using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetTodayEvents
{
    public class EmployeeGetTodayEventsQuery : IRequest<List<TodayEventResponse>>
    {
    }
}
