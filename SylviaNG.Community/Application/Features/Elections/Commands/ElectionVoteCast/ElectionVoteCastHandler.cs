using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionVoteCast
{
    public class ElectionVoteCastHandler : IRequestHandler<ElectionVoteCastCommand, List<long>>
    {
        private readonly IElectionService _electionService;

        public ElectionVoteCastHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task<List<long>> Handle(ElectionVoteCastCommand command, CancellationToken cancellationToken)
        {
            return await _electionService.CastVoteAsync(command.ElectionId, command.Request, command.VoterId);
        }
    }
}
