using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCandidateApprove
{
    public class ElectionCandidateApproveHandler : IRequestHandler<ElectionCandidateApproveCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionCandidateApproveHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionCandidateApproveCommand command, CancellationToken cancellationToken)
        {
            await _electionService.ApproveCandidateAsync(command.ElectionId, command.CandidateId);
        }
    }
}
