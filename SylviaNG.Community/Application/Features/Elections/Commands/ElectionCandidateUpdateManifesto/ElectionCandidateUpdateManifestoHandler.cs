using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateUpdateManifesto
{
    public class ElectionCandidateUpdateManifestoHandler : IRequestHandler<ElectionCandidateUpdateManifestoCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionCandidateUpdateManifestoHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionCandidateUpdateManifestoCommand command, CancellationToken cancellationToken)
        {
            await _electionService.UpdateManifestoAsync(command.ElectionId, command.CandidateId, command.Request);
        }
    }
}
