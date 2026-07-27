using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateNominate
{
    public class ElectionCandidateNominateHandler : IRequestHandler<ElectionCandidateNominateCommand, long>
    {
        private readonly IElectionService _electionService;

        public ElectionCandidateNominateHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<long> Handle(ElectionCandidateNominateCommand command, CancellationToken cancellationToken)
        {
            return await _electionService.NominateAsync(command.ElectionId, command.Request);
        }
    }
}
