using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateRemove
{
    public class ElectionCandidateRemoveHandler : IRequestHandler<ElectionCandidateRemoveCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionCandidateRemoveHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionCandidateRemoveCommand command, CancellationToken cancellationToken)
        {
            await _electionService.RemoveCandidateAsync(command.ElectionId, command.CandidateId);
        }
    }
}
