using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Interests.Commands.InterestDelete
{
    public class InterestDeleteHandler : IRequestHandler<InterestDeleteCommand>
    {
        private readonly IInterestService _interestService;

        public InterestDeleteHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task Handle(InterestDeleteCommand command, CancellationToken cancellationToken)
        {
            await _interestService.DeleteAsync(command.InterestId);
        }
    }
}
