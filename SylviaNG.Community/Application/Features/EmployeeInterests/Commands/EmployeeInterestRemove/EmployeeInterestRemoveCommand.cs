using MediatR;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestRemove
{
    public class EmployeeInterestRemoveCommand : IRequest
    {
        public long EmployeeId { get; set; }
        public long InterestId { get; set; }

        public EmployeeInterestRemoveCommand(long employeeId, long interestId)
        {
            EmployeeId = employeeId;
            InterestId = interestId;
        }
    }
}
