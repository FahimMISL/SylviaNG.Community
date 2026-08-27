using MediatR;
using SylviaNG.Community.Application.Features.Employees.Models;

namespace SylviaNG.Community.Application.Features.Employees.Queries.EmployeeGetNewJoinees
{
    public class EmployeeGetNewJoineesQuery : IRequest<List<NewJoineeResponse>>
    {
    }
}
