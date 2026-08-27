using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionDelete
{
    public class ElectionDeleteHandler : IRequestHandler<ElectionDeleteCommand>
    {
        private readonly IElectionService _electionService;

        public ElectionDeleteHandler(IElectionService electionService)
        {
            _electionService = electionService;
        }

        public async Task Handle(ElectionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _electionService.DeleteAsync(command.ElectionId);
        }
    }
}
