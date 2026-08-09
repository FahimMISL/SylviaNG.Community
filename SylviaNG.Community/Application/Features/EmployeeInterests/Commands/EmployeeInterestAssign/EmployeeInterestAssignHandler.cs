using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestAssign
{
    public class EmployeeInterestAssignHandler : IRequestHandler<EmployeeInterestAssignCommand, long>
    {
        private readonly IInterestService _interestService;

        public EmployeeInterestAssignHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task<long> Handle(EmployeeInterestAssignCommand command, CancellationToken cancellationToken)
        {
            return await _interestService.AssignToEmployeeAsync(command.EmployeeId, command.Request);
        }
    }
}
