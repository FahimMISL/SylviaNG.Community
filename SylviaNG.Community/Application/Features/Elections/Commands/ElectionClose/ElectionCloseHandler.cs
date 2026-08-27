using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionClose
{
    public class ElectionCloseHandler : IRequestHandler<ElectionCloseCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionCloseHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionCloseCommand command, CancellationToken cancellationToken)
        {
            await _electionService.CloseAsync(command.ElectionId);
        }
    }
}
