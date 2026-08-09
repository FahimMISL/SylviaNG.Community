using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionUpdate
{
    public class ElectionUpdateHandler : IRequestHandler<ElectionUpdateCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionUpdateHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _electionService.UpdateAsync(command.ElectionId, command.Request);
        }
    }
}
