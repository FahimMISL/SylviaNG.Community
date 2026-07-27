using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeInterests.Commands.EmployeeInterestRemove
{
    public class EmployeeInterestRemoveHandler : IRequestHandler<EmployeeInterestRemoveCommand>
    {
        private readonly IInterestService _interestService;

        public EmployeeInterestRemoveHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task Handle(EmployeeInterestRemoveCommand command, CancellationToken cancellationToken)
        {
            await _interestService.RemoveFromEmployeeAsync(command.EmployeeId, command.InterestId);
        }
    }
}
