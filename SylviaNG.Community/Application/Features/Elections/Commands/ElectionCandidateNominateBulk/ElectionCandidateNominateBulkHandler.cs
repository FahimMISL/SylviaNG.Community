using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominateBulk
{
    public class ElectionCandidateNominateBulkHandler : IRequestHandler<ElectionCandidateNominateBulkCommand, int>
    {
        private readonly IElectionService _electionService;

        public ElectionCandidateNominateBulkHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<int> Handle(ElectionCandidateNominateBulkCommand command, CancellationToken cancellationToken)
        {
            return await _electionService.NominateBulkAsync(command.ElectionId, command.Request);
        }
    }
}
