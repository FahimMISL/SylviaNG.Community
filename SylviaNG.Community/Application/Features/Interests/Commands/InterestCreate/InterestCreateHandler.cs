using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Interests.Commands.InterestCreate
{
    public class InterestCreateHandler : IRequestHandler<InterestCreateCommand, long>
    {
        private readonly IInterestService _interestService;

        public InterestCreateHandler(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public async Task<long> Handle(InterestCreateCommand command, CancellationToken cancellationToken)
        {
            return await _interestService.CreateAsync(command.Request);
        }
    }
}
