using MediatR;
using SylviaNG.Community.Application.Features.EmployeeInterests.Models;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Queries.EmployeeInterestGetAll
{
    public class EmployeeInterestGetAllQuery : IRequest<List<EmployeeInterestResponse>>
    {
        public long EmployeeId { get; set; }

        public EmployeeInterestGetAllQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
