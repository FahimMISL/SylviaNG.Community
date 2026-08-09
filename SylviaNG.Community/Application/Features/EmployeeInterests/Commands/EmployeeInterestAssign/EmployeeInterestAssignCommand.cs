using MediatR;
using SylviaNG.Community.Application.Features.EmployeeInterests.Models;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestAssign
{
    public class EmployeeInterestAssignCommand : IRequest<long>
    {
        public long EmployeeId { get; set; }
        public EmployeeInterestAssignRequest Request { get; set; }

        public EmployeeInterestAssignCommand(long employeeId, EmployeeInterestAssignRequest request)
        {
            EmployeeId = employeeId;
            Request = request;
        }
    }
}
