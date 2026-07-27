using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionAudienceTargetAdd
{
    public class ElectionAudienceTargetAddHandler : IRequestHandler<ElectionAudienceTargetAddCommand, long>
    {
        private readonly IElectionService _electionService;

        public ElectionAudienceTargetAddHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<long> Handle(ElectionAudienceTargetAddCommand command, CancellationToken cancellationToken)
        {
            return await _electionService.AddAudienceTargetAsync(command.ElectionId, command.Request);
        }
    }
}
